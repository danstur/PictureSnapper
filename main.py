import cv2


def main():
    vc = cv2.VideoCapture(0)
    if vc.isOpened():
        print('all good?')
    else:
        print('???')


if __name__ == '__main__':
    main()
