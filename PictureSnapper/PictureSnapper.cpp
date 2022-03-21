#define WIN32_LEAN_AND_MEAN 
#define VC_EXTRALEAN
#include <Windows.h>
#include <opencv2/opencv.hpp>
#include <iostream>
#include <string>

void grab_picture() {

}

int wmain(int argc, wchar_t *argv[])
{
    if (argc != 2) {
        std::cout << "Pass directory as only argument\n";
        return 1;
    }
    auto dir = argv[1];
    auto result = GetFileAttributesW(dir);
    if (result == INVALID_FILE_ATTRIBUTES) {
        std::cout << "Directory does not exist.\n";
        return 1;
    }

    auto capture = cv::VideoCapture(0);
    if (!capture.isOpened()) {
        std::cout << "Could not find camera\n";
        return 1;
    }
    cv::Mat frame;
    capture.read(frame);
    if (frame.empty()) {
        std::cout << "Could not read frame :(\n";
        return 1;
    }
    auto success = cv::imwrite(R"(C:\tmp\test.jpeg)", frame);
    if (!success) {
        std::cout << "Could not save picture\n";
        return 1;
    }
    return 0;
}
