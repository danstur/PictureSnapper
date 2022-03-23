//#define WIN32_LEAN_AND_MEAN 
//#define VC_EXTRALEAN

#include <Objbase.h>
#include <vector>
#include <opencv2/opencv.hpp>

extern "C" __declspec(dllexport)
bool __stdcall grab_image(char** image, int* imageSize) {
    auto capture = cv::VideoCapture(0);
    if (!capture.isOpened()) {
        return false;
    }
    cv::Mat frame;
    capture.read(frame);
    if (frame.empty()) {
        return false;
    }
    std::vector<uchar> encoded_image;
    if (!cv::imencode(".jpg", frame, encoded_image)) {
        return false;
    }
    auto marshalled_data = (char*)::CoTaskMemAlloc(encoded_image.size());
    if (!marshalled_data) {
        return false;
    }
    std::copy(encoded_image.begin(), encoded_image.end(), marshalled_data);
    *imageSize = encoded_image.size();
    *image = marshalled_data;
    return true;
}

