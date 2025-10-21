import React, { useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X, Camera as CameraIcon } from 'lucide-react';

const CameraModal = ({ isOpen, onClose, videoRef, onCapture }) => {
  useEffect(() => {
    if (!isOpen && videoRef.current && videoRef.current.srcObject) {
      const stream = videoRef.current.srcObject;
      stream.getTracks().forEach(track => track.stop());
    }
  }, [isOpen, videoRef]);

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/90 backdrop-blur-sm p-4"
          onClick={onClose}
        >
          <motion.div
            initial={{ scale: 0.9, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            exit={{ scale: 0.9, opacity: 0 }}
            className="gothic-card p-4 md:p-6 max-w-2xl w-full"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="flex justify-between items-center mb-4">
              <h3 className="text-xl font-chinese text-gold">拍照 Camera</h3>
              <button
                onClick={onClose}
                className="p-2 hover:bg-crimson/20 rounded-lg transition-colors"
              >
                <X className="w-5 h-5 text-silver" />
              </button>
            </div>

            <div className="relative bg-black-velvet rounded-lg overflow-hidden mb-4">
              <video
                ref={videoRef}
                autoPlay
                playsInline
                className="w-full h-auto max-h-96"
              />
            </div>

            <button
              onClick={onCapture}
              className="gothic-button w-full flex items-center justify-center space-x-2"
            >
              <CameraIcon className="w-5 h-5" />
              <span>拍照 Capture</span>
            </button>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
};

export default CameraModal;
