//#define WIN32_LEAN_AND_MEAN 
//#define VC_EXTRALEAN

#include <Objbase.h>
#include <vector>
#include <opencv2/opencv.hpp>

extern "C" __declspec(dllexport)
void __stdcall grab_image(char** image, int* imageSize) {
    auto capture = cv::VideoCapture(0);
    if (!capture.isOpened()) {
        std::cout << "Could not find camera\n";
        return;
    }
    cv::Mat frame;
    capture.read(frame);
    if (frame.empty()) {
        std::cout << "Could not read frame :(\n";
        return;
    }
    std::vector<uchar> encoded_image;
    if (!cv::imencode(".jpg", frame, encoded_image)) {
        std::cout << "Could not encode frame :(\n";
        return;
    }
    auto marshalled_data = (char*)::CoTaskMemAlloc(encoded_image.size());
    if (!marshalled_data) {
        std::cout << "Could not allocate memory :(\n";
        return;
    }
    std::copy(encoded_image.begin(), encoded_image.end(), marshalled_data);
    *imageSize = encoded_image.size();
    *image = marshalled_data;
}



//void CALLBACK grab_picture(void* param, BOOLEAN) {
//    auto dir = (wchar_t*)param;
//    std::time_t t = std::time(nullptr);
//    std::tm tm;
//    auto error = ::localtime_s(&tm, &t);
//    if (error != 0) {
//        throw std::exception("Could not get local date time.");
//    }
//    std::stringstream ss;
//    ss << std::put_time(&tm, "%F_%H-%M-%S");
//    auto current_time = ss.str();
//    std::cout << "Grab picture at " << current_time << "\n";
//    auto capture = cv::VideoCapture(0);
//    if (!capture.isOpened()) {
//        std::cout << "Could not find camera\n";
//        return;
//    }
//    cv::Mat frame;
//    capture.read(frame);
//    if (frame.empty()) {
//        std::cout << "Could not read frame :(\n";
//        return;
//    }
//    std::wstring img_path;
//    
//    auto success = cv::imwrite(R"(C:\tmp\test.jpeg)", frame);
//    if (!success) {
//        std::cout << "Could not save picture\n";
//        return;
//    }
//}
//
//int wmain(int argc, wchar_t *argv[])
//{
//    if (argc != 2) {
//        std::cout << "Pass directory as only argument\n";
//        return 1;
//    }
//    auto dir = argv[1];
//    auto result = GetFileAttributesW(dir);
//    if (result == INVALID_FILE_ATTRIBUTES) {
//        std::cout << "Directory does not exist.\n";
//        return 1;
//    }
//    
//    HANDLE timer;
//    auto success = CreateTimerQueueTimer(&timer, nullptr, grab_picture, dir, 1000, 1000, WT_EXECUTEDEFAULT);
//    if (success == 0) {
//        std::cout << "CreateTimerQueueTimer failed.\n";
//        return 1;
//    }
//    system("pause");
//    std::cout << "finished\n";
//    return 0;
//}
