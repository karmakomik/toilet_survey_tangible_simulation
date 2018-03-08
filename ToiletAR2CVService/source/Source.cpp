#include <windows.h>
#include <chilitags.hpp>
#include <iostream>
using namespace std;

//#ifdef OPENCV3
#include <opencv2/core/utility.hpp> // getTickCount...
#include <opencv2/imgproc/imgproc.hpp>
//#endif

#include <opencv2/core/core_c.h> // CV_AA
#include <opencv2/highgui/highgui.hpp>

unsigned long __stdcall ReadMessageFromPipe(void * pParam);
int convMsg;
char recvdMsg[100];
char copyRcvdMsg[100];

DWORD WINAPI ThreadProc();
HANDLE hPipe1, hPipe2, hPipe3;
BOOL Finished;

bool isPerspMatCalculated = false;
cv::Mat perspMat;
cv::Mat warpedIm;

// Not much interesting here, move along
void drawTags(cv::Mat outputImage, const std::map<int, chilitags::Quad> &tags);

void correctPerspective(cv::Mat initImage, chilitags::Chilitags& detectedChilitags);
void sortCorners(std::vector<cv::Point2f>& corners, cv::Point2f center);

bool isUnityAppActive = true;

int main(int argc, char* argv[])
{
	// Initialising input video
	int xRes = 800;
	int yRes = 600;
	int cameraIndex = 0;
	if (argc > 2) 
	{
		xRes = std::atoi(argv[1]);
		yRes = std::atoi(argv[2]);
	}
	if (argc > 3) 
	{
		cameraIndex = std::atoi(argv[3]);
	}

	// The source of input images
	cv::VideoCapture capture(cameraIndex);
	if (!capture.isOpened())
	{
		std::cerr << "Unable to initialise video capture." << std::endl;
		return 1;
	}
#ifdef OPENCV3
	capture.set(cv::CAP_PROP_FRAME_WIDTH, xRes);
	capture.set(cv::CAP_PROP_FRAME_HEIGHT, yRes);
#else
	capture.set(CV_CAP_PROP_FRAME_WIDTH, xRes);
	capture.set(CV_CAP_PROP_FRAME_HEIGHT, yRes);
#endif
	cv::Mat inputImage;

	// We need separate Chilitags if we want to compare find() with different
	// detection/tracking parameters on the same image

	// This one is the reference Chilitags
	chilitags::Chilitags detectedChilitags;
	detectedChilitags.setFilter(0, 0.);

	// This one will be called with JUST_TRACK when it has previously detected
	// something
	//chilitags::Chilitags trackedChilitags;
	//trackedChilitags.setFilter(0, 0.);

	cv::namedWindow("DisplayChilitags");
	//cv::namedWindow("Warped");

	std::map<int, chilitags::Quad> warpedImDetectedTags;
	//cv::namedWindow("Text Subregion");

	// Do we want to run and show the reference detection ?
	bool showReference = true;
	// Do we want to run and show the tracking-based detection ?
	bool showTracking = true;

	// In the tracking-based detection, we need to know whether there is
	// something to track
	bool tracking = false;

	char keyPressed;

	float tag1Angle;
	cv::Point2f tag1Center;

	//Pipe Init Data
	char buf[100];

	LPTSTR lpszPipename1 = TEXT("\\\\.\\pipe\\myNamedPipe1");
	LPTSTR lpszPipename2 = TEXT("\\\\.\\pipe\\myNamedPipe2");

	DWORD cbWritten;
	strcpy(buf, "<0:0000:0000:0000:0000:0000>"); //Format : <tagnum:ang:centerloc_x:centerloc_y:length>
	cout << "template length : " << strlen(buf) << endl;
	DWORD dwBytesToWrite = (DWORD)strlen(buf);

	//Thread Init Data  
	DWORD threadId;
	HANDLE hThread = NULL;

	BOOL Write_St = TRUE;

	Finished = FALSE;

	hPipe1 = CreateFile(lpszPipename1, GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
	hPipe2 = CreateFile(lpszPipename2, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);


	/*if ((hPipe1 == NULL || hPipe1 == INVALID_HANDLE_VALUE))
	{
		printf("Could not open the pipe  - (error %d)\n", GetLastError());

	}
	else
	{*/
		short count = 0;
		hThread = CreateThread(NULL, 0, &ReadMessageFromPipe, NULL, 0, NULL);
		while ('q' != (keyPressed = (char)cv::waitKey(1)) && isUnityAppActive)
		{

			// toggle the processing, according to user input
			//if (keyPressed == 't') showTracking = !showTracking;
			//if (keyPressed == 'd') showReference = !showReference;

			capture.read(inputImage);
			cv::rotate(inputImage, inputImage, 1);

			cv::Mat outputImage = inputImage.clone();
			//cv::Mat textSubImage = inputImage.clone();


			if (keyPressed == 'p')
			{
				correctPerspective(inputImage, detectedChilitags);
			}

			if (isPerspMatCalculated)
			{
				cv::warpPerspective(inputImage, warpedIm, perspMat, warpedIm.size());
				warpedImDetectedTags = detectedChilitags.find(warpedIm);
				drawTags(warpedIm, warpedImDetectedTags);
				cv::imshow("Warped", warpedIm);
				//warpedImDetectedTags = detectedChilitags.find(warpedIm);
			}

			// nothing new here
			if (showReference) 
			{
				int64 startTime = cv::getTickCount();
				auto tags = detectedChilitags.find(inputImage);

				//cout << "Number of detected tags : " << tags.size() << endl;

				int64 endTime = cv::getTickCount();
				drawTags(outputImage, tags);
			
				for (const auto & tag : tags)
				{
					const cv::Mat_<cv::Point2f> corners(tag.second);

					tag1Center = 0.5*(corners(0) + corners(2));
					cout << "Tag id - " << tag.first << endl;
					//cout << "Loc x : " << tag1Center.x << ", y : " << tag1Center.y << endl;

					cv::Point2f pt1 = corners(0);
					cv::Point2f pt2 = corners(1);
					tag1Angle = (57.3 * atan2((pt1.y - pt2.y), (pt1.x - pt2.x)));
					double length = cv::norm(pt1 - pt2);
					//cout << "buf length : " <<  << endl;

					sprintf(buf, "<%d:%0004d:%0004d:%0004d:%0004d:%0004d>", (count%2),tag.first, (int)tag1Angle, (int)tag1Center.x, (int)tag1Center.y, (int)length); //Format : <tagnum:ang:centerloc_x:centerloc_y:length>
					
					/*if (tag.first == 5)
					{
						//cout << "Tag 4 found " << endl;
						cv::Mat rot_mat = cv::getRotationMatrix2D(cv::Point2f(tag1Center.x, tag1Center.y), tag1Angle - 180, 1);
						cv::warpAffine(inputImage, textSubImage, rot_mat, inputImage.size(), cv::INTER_CUBIC); //http://answers.opencv.org/question/497/extract-a-rotatedrect-area/ & http://felix.abecassis.me/2011/10/opencv-rotation-deskewing/
						// crop the resulting image
						//cv::getRectSubPix(textSubImage, cv::Size(length, length), tag1Center, textSubImage);
						textSubImage = textSubImage(cv::Rect((int)tag1Center.x - (int)length/2 - (int)length - (int)length/4, (int)tag1Center.y - (int)length / 2, (int)length, (int)length));
						//cv::cvtColor(textSubImage, textSubImage, cv::COLOR_RGB2GRAY);
						cv::inRange(textSubImage, cv::Scalar(0, 0, 0), cv::Scalar(110, 255, 255), textSubImage);
						cv::threshold(textSubImage, textSubImage, 0, 255, cv::THRESH_BINARY);
					}*/
																																									 //WRITE INTO PIPE
					WriteFile(hPipe1, buf, strlen(buf), &cbWritten, NULL);
					memset(buf, 0xCC, 100);
					/*if (tag.first == 2)
					cout << tag1Angle << "," << tag1Center.y << endl;*/
				}
			
			}
			++count;

			/*if (showTracking) 
			{
				int64 startTime = cv::getTickCount();
				// Tracking needs something to track; it is initialised with a
				// regular detection (find()). When something is detected, tracking
				// will take over and return tags processed from the previous call
				// to track() as long as there is something returned.
				// When nothing is returned, we are back to regular detection.
				auto tags =
					trackedChilitags.find(inputImage, tracking ? chilitags::Chilitags::TRACK_ONLY : chilitags::Chilitags::TRACK_AND_DETECT);

				int64 endTime = cv::getTickCount();
				drawTags(outputImage, tags, startTime, endTime, false);
				tracking = !tags.empty();
			}*/

			cv::imshow("DisplayChilitags", outputImage);
			//
			//cv::imshow("Text Subregion", textSubImage);

		}
	//}

	cv::destroyWindow("DisplayChilitags");
	cv::destroyWindow("Text Subregion");
	capture.release();

	return 0;
}

unsigned long __stdcall ReadMessageFromPipe(void * pParam)
{
	BOOL fSuccess;
	//char chBuf[100];
	//DWORD dwBytesToWrite = (DWORD)strlen(chBuf);

	DWORD cbRead;
	int i;
	//string delimiter = "<"

	while (1)
	{
		try
		{
			//cout << "inside!" << endl;
			fSuccess = ReadFile(hPipe2, recvdMsg,/*dwBytesToRead*/ 22, &cbRead, NULL);
			strcpy(copyRcvdMsg, recvdMsg);
			cout << "recvd msg : " << recvdMsg << endl;
			//string msg(recvdMsg);

			if (fSuccess)
			{
				if (!strncmp(recvdMsg, "quit", 4))
				{
					cout << "Quit message recvd" << endl;
					isUnityAppActive = false;
				}
				if (cbRead == 5)
				{
					//printf("C++ App: Received %d Bytes : ",cbRead);
					//for(i=0;i<cbRead;i++)
					//	printf("%c",chBuf[i]);
				}
				//printf("\n");
			}
			if (!fSuccess && GetLastError() != ERROR_MORE_DATA)
			{
				printf("Can't Read\n");
				if (Finished)
					break;
			}
		}
		catch (...)
		{
			cout << "Exception thrown" << endl;
		}
	}
	return 1;
}

void drawTags(cv::Mat outputImage, const std::map<int, chilitags::Quad> &tags)
{
	//Adapted from : https://github.com/chili-epfl/chilitags/blob/master/samples/tracking/tracking.cpp

	cv::Scalar COLOR = cv::Scalar(255, 0, 0);

	for (const auto & tag : tags) {

		const cv::Mat_<cv::Point2f> corners(tag.second);

		for (size_t i = 0; i < 4; ++i) {
			static const int SHIFT = 16;
			static const float PRECISION = 1 << SHIFT;
			cv::line(
				outputImage,
				PRECISION*corners(i),
				PRECISION*corners((i + 1) % 4),
#ifdef OPENCV3
				COLOR, detection ? 3 : 1, cv::LINE_AA, SHIFT);
#else
				COLOR, 1, CV_AA, SHIFT);
#endif
		}

		cv::Point tag1Center = 0.5*(corners(0) + corners(2));
		/*
		//cout << "Loc : " << tag1Center.x << "," << tag1Center.y << endl;

		cv::Point2f pt1 = corners(0);
		cv::Point2f pt2 = corners(1);
		tag1Angle = (57.3 * atan((pt1.y - pt2.y) / (pt1.x - pt2.x)));
		//cout << "Ang : " << tag1Angle << endl;*/


		//tag.second.

		cv::putText(outputImage, cv::format("%d", tag.first), tag1Center,
			cv::FONT_HERSHEY_SIMPLEX, 0.5, COLOR);
		//cout << "tag id - " << tag.first << endl; //////***********This is how you get the Tag id (a number)!
	}
}

void correctPerspective(cv::Mat initImage, chilitags::Chilitags& detectedChilitags)
{
	auto tags = detectedChilitags.find(initImage);
	//drawTags(initImage, tags);

	//if (keyPressed == 'p')
	//{
	std::vector<cv::Point2f> destPts;
	std::vector<cv::Point2f> srcPts;
	//srcPts.resize(4);
	cv::Point2f bottomLeft, bottomRight, topLeft, topRight;

	for (auto tag : tags)
	{
		chilitags::Quad tagQuad = tag.second;
		//tagNum = tag.first;

		const cv::Mat_<cv::Point2f> corners(tagQuad);
		std::vector<cv::Point2f> cornersList;
		cv::Point2f currentTagPos = 0.5 * (corners(0) + corners(2));
		if (tag.first == 164 || tag.first == 37 || tag.first == 163 || tag.first == 166)
		{
			//srcPts.push_back(currentTagPos);
			cornersList.push_back(corners(0));
			cornersList.push_back(corners(1));
			cornersList.push_back(corners(2));
			cornersList.push_back(corners(3));
			sortCorners(cornersList, currentTagPos);

		}
		if (tag.first == 163)
		{
			bottomRight = cornersList[0];
			srcPts.push_back(bottomRight);
		}
		if (tag.first == 164)
		{
			topLeft = cornersList[2];
			srcPts.push_back(topLeft);
		}
		if (tag.first == 37)
		{
			bottomLeft = cornersList[1];
			srcPts.push_back(bottomLeft);
		}
		if (tag.first == 166)
		{
			topRight = cornersList[3];
			srcPts.push_back(topRight);
		}
		//cout << "currentTagPos" << currentTagPos << endl;
	}
	// Get mass center
	cv::Point2f center(0, 0);
	for (int i = 0; i < srcPts.size(); i++)
	{
		center += srcPts[i];
	}

	center *= (1. / srcPts.size());
	sortCorners(srcPts, center);

	float ratio = abs(srcPts[0].x - srcPts[1].x) / (float)abs(srcPts[1].y - srcPts[2].y);
	cout << "ratio : " << ratio << endl;
	warpedIm = cv::Mat::zeros(ceil(800 / ratio), 800, CV_8UC3);

	//beginAreaX = 0;
	//endAreaX = warpedIm.cols;
	//beginAreaY = 0;
	//endAreaY = warpedIm.rows;

	//workPieceBoundingBox = Iso_rectangle_2(Point_2(beginAreaX, beginAreaY), Point_2(endAreaX, endAreaY));

	//totalArea = endAreaX * endAreaY;

	destPts.push_back(cv::Point2f(0, 0));
	destPts.push_back(cv::Point2f(warpedIm.cols, 0));
	destPts.push_back(cv::Point2f(warpedIm.cols, warpedIm.rows));
	destPts.push_back(cv::Point2f(0, warpedIm.rows));
	//cout << "destPts size" << destPts.size() << "srcPts size" << srcPts.size() << endl;

	if (srcPts.size() == 4)
	{
		perspMat = cv::getPerspectiveTransform(srcPts, destPts);
		isPerspMatCalculated = true;
		//ratio = 
	}
	else
	{
		cout << "Missing corner markers!" << endl;
	}
	//}
}

void sortCorners(std::vector<cv::Point2f>& corners, cv::Point2f center)
{
	//Adapted from : http://opencv-code.com/tutorials/automatic-perspective-correction-for-quadrilateral-objects/

	std::vector<cv::Point2f> top, bot;

	for (int i = 0; i < corners.size(); i++)
	{
		if (corners[i].y < center.y)
			top.push_back(corners[i]);
		else
			bot.push_back(corners[i]);
	}

	cv::Point2f tl = top[0].x > top[1].x ? top[1] : top[0];
	cv::Point2f tr = top[0].x > top[1].x ? top[0] : top[1];
	cv::Point2f bl = bot[0].x > bot[1].x ? bot[1] : bot[0];
	cv::Point2f br = bot[0].x > bot[1].x ? bot[0] : bot[1];

	corners.clear();
	corners.push_back(tl);
	corners.push_back(tr);
	corners.push_back(br);
	corners.push_back(bl);
}
