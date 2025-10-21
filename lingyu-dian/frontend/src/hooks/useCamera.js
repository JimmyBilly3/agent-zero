import { useState, useRef, useCallback } from 'react';

export const useCamera = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [capturedImage, setCapturedImage] = useState(null);
  const videoRef = useRef(null);
  const streamRef = useRef(null);

  const openCamera = useCallback(async () => {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: 'environment' }
      });
      streamRef.current = stream;
      if (videoRef.current) {
        videoRef.current.srcObject = stream;
      }
      setIsOpen(true);
    } catch (error) {
      console.error('Error opening camera:', error);
      throw error;
    }
  }, []);

  const closeCamera = useCallback(() => {
    if (streamRef.current) {
      streamRef.current.getTracks().forEach(track => track.stop());
      streamRef.current = null;
    }
    setIsOpen(false);
  }, []);

  const captureImage = useCallback(() => {
    if (videoRef.current) {
      const canvas = document.createElement('canvas');
      canvas.width = videoRef.current.videoWidth;
      canvas.height = videoRef.current.videoHeight;
      const ctx = canvas.getContext('2d');
      ctx.drawImage(videoRef.current, 0, 0);

      canvas.toBlob((blob) => {
        setCapturedImage(blob);
        closeCamera();
      }, 'image/jpeg');
    }
  }, [closeCamera]);

  const resetImage = useCallback(() => {
    setCapturedImage(null);
  }, []);

  return {
    isOpen,
    capturedImage,
    videoRef,
    openCamera,
    closeCamera,
    captureImage,
    resetImage,
  };
};
