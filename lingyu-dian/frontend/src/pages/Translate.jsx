import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Mic, Camera, Volume2, Loader2, Wifi, WifiOff, Copy, Check } from 'lucide-react';
import useStore from '../store/useStore';
import LanguageSelector from '../components/LanguageSelector';
import ToneSelector from '../components/ToneSelector';
import CameraModal from '../components/CameraModal';
import { useAudioRecorder } from '../hooks/useAudioRecorder';
import { useCamera } from '../hooks/useCamera';
import { translateText, speechToText, textToSpeech, ocrImage, checkOnlineStatus } from '../utils/api';
import { saveTranslation } from '../utils/storage';

const Translate = () => {
  const {
    settings,
    inputText,
    outputText,
    isTranslating,
    isOnline,
    setInputText,
    setOutputText,
    setIsTranslating,
    setIsOnline,
  } = useStore();

  const [copied, setCopied] = useState(false);
  const [error, setError] = useState(null);
  const [isPlayingAudio, setIsPlayingAudio] = useState(false);

  const {
    isRecording,
    audioBlob,
    startRecording,
    stopRecording,
    resetRecording,
  } = useAudioRecorder();

  const {
    isOpen: isCameraOpen,
    capturedImage,
    videoRef,
    openCamera,
    closeCamera,
    captureImage,
    resetImage,
  } = useCamera();

  // Check online status
  useEffect(() => {
    const handleOnline = () => setIsOnline(true);
    const handleOffline = () => setIsOnline(false);

    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    return () => {
      window.removeEventListener('online', handleOnline);
      window.removeEventListener('offline', handleOffline);
    };
  }, [setIsOnline]);

  // Handle translation
  const handleTranslate = async () => {
    if (!inputText.trim()) return;

    setIsTranslating(true);
    setError(null);

    try {
      const result = await translateText(
        inputText,
        settings.sourceLang,
        settings.targetLang,
        settings.tone,
        settings.offlineMode || !isOnline
      );

      setOutputText(result.translated_text);

      // Save to history
      await saveTranslation({
        sourceText: inputText,
        translatedText: result.translated_text,
        sourceLang: settings.sourceLang,
        targetLang: settings.targetLang,
        tone: settings.tone,
      });

      // Play chime sound
      playChime();
    } catch (err) {
      console.error('Translation error:', err);
      setError('Translation failed. Please try again.');
    } finally {
      setIsTranslating(false);
    }
  };

  // Handle speech recognition
  useEffect(() => {
    if (audioBlob) {
      handleSpeechToText();
    }
  }, [audioBlob]);

  const handleSpeechToText = async () => {
    try {
      setIsTranslating(true);
      const result = await speechToText(audioBlob, settings.sourceLang);
      setInputText(result.text);
      resetRecording();
    } catch (err) {
      console.error('Speech to text error:', err);
      setError('Speech recognition failed. Please try again.');
    } finally {
      setIsTranslating(false);
    }
  };

  // Handle OCR
  useEffect(() => {
    if (capturedImage) {
      handleOCR();
    }
  }, [capturedImage]);

  const handleOCR = async () => {
    try {
      setIsTranslating(true);
      const result = await ocrImage(capturedImage);
      setInputText(result.text);
      resetImage();
    } catch (err) {
      console.error('OCR error:', err);
      setError('Image text recognition failed. Please try again.');
    } finally {
      setIsTranslating(false);
    }
  };

  // Handle text to speech
  const handleTextToSpeech = async () => {
    if (!outputText) return;

    try {
      setIsPlayingAudio(true);
      const audioBlob = await textToSpeech(outputText, settings.targetLang, settings.voiceGender);
      const audioUrl = URL.createObjectURL(audioBlob);
      const audio = new Audio(audioUrl);

      audio.onended = () => {
        setIsPlayingAudio(false);
        URL.revokeObjectURL(audioUrl);
      };

      await audio.play();
    } catch (err) {
      console.error('Text to speech error:', err);
      setError('Voice playback failed. Please try again.');
      setIsPlayingAudio(false);
    }
  };

  // Copy to clipboard
  const handleCopy = () => {
    if (outputText) {
      navigator.clipboard.writeText(outputText);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    }
  };

  // Play chime sound
  const playChime = () => {
    const audioContext = new (window.AudioContext || window.webkitAudioContext)();
    const oscillator = audioContext.createOscillator();
    const gainNode = audioContext.createGain();

    oscillator.connect(gainNode);
    gainNode.connect(audioContext.destination);

    oscillator.frequency.value = 800;
    oscillator.type = 'sine';

    gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
    gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.5);

    oscillator.start(audioContext.currentTime);
    oscillator.stop(audioContext.currentTime + 0.5);
  };

  return (
    <div className="max-w-4xl mx-auto">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="gothic-card p-6 md:p-8"
      >
        {/* Online/Offline Status */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center space-x-2">
            {isOnline ? (
              <>
                <Wifi className="w-4 h-4 text-green-500" />
                <span className="text-xs text-green-500">Online</span>
              </>
            ) : (
              <>
                <WifiOff className="w-4 h-4 text-red-500" />
                <span className="text-xs text-red-500">Offline</span>
              </>
            )}
          </div>
          <div className="text-xs text-silver/60">
            Mode: {settings.offlineMode ? 'Offline' : 'Online'}
          </div>
        </div>

        {/* Language Selector */}
        <div className="mb-6">
          <LanguageSelector />
        </div>

        {/* Tone Selector */}
        <div className="mb-6">
          <ToneSelector />
        </div>

        {/* Input Text Area */}
        <div className="mb-4">
          <label className="block text-gold text-sm font-gothic mb-2">
            源文本 Source Text
          </label>
          <textarea
            value={inputText}
            onChange={(e) => setInputText(e.target.value)}
            placeholder="Enter text to translate..."
            className="gothic-input w-full h-32 md:h-40 resize-none"
            onKeyDown={(e) => {
              if (e.key === 'Enter' && e.ctrlKey) {
                handleTranslate();
              }
            }}
          />
        </div>

        {/* Action Buttons */}
        <div className="flex flex-wrap justify-center gap-3 mb-6">
          <motion.button
            onClick={isRecording ? stopRecording : startRecording}
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            className={`p-3 rounded-full border-2 transition-all duration-300 ${
              isRecording
                ? 'bg-red-500 border-red-500 animate-pulse'
                : 'bg-black-velvet border-gold/30 hover:border-gold'
            }`}
            title="Voice Input"
          >
            <Mic className={`w-5 h-5 ${isRecording ? 'text-white' : 'text-gold'}`} />
          </motion.button>

          <motion.button
            onClick={openCamera}
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            className="p-3 rounded-full bg-black-velvet border-2 border-gold/30 hover:border-gold transition-all duration-300"
            title="Camera OCR"
          >
            <Camera className="w-5 h-5 text-gold" />
          </motion.button>
        </div>

        {/* Translate Button */}
        <motion.button
          onClick={handleTranslate}
          disabled={isTranslating || !inputText.trim()}
          whileHover={{ scale: isTranslating ? 1 : 1.02 }}
          whileTap={{ scale: isTranslating ? 1 : 0.98 }}
          className="gothic-button w-full mb-6 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isTranslating ? (
            <span className="flex items-center justify-center space-x-2">
              <Loader2 className="w-5 h-5 animate-spin" />
              <span>翻譯中 Translating...</span>
            </span>
          ) : (
            <span>翻譯 Translate</span>
          )}
        </motion.button>

        {/* Error Message */}
        {error && (
          <motion.div
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            className="mb-4 p-3 bg-red-500/10 border border-red-500/30 rounded-lg text-red-400 text-sm"
          >
            {error}
          </motion.div>
        )}

        {/* Output Text Area */}
        <div className="mb-4">
          <div className="flex items-center justify-between mb-2">
            <label className="text-gold text-sm font-gothic">
              譯文 Translated Text
            </label>
            <div className="flex space-x-2">
              <motion.button
                onClick={handleTextToSpeech}
                disabled={!outputText || isPlayingAudio}
                whileHover={{ scale: 1.1 }}
                whileTap={{ scale: 0.9 }}
                className="p-2 rounded-full bg-black-velvet border border-gold/30 hover:border-gold transition-all disabled:opacity-50"
                title="Play Audio"
              >
                <Volume2 className={`w-4 h-4 text-gold ${isPlayingAudio ? 'animate-pulse' : ''}`} />
              </motion.button>
              <motion.button
                onClick={handleCopy}
                disabled={!outputText}
                whileHover={{ scale: 1.1 }}
                whileTap={{ scale: 0.9 }}
                className="p-2 rounded-full bg-black-velvet border border-gold/30 hover:border-gold transition-all disabled:opacity-50"
                title="Copy to Clipboard"
              >
                {copied ? (
                  <Check className="w-4 h-4 text-green-500" />
                ) : (
                  <Copy className="w-4 h-4 text-gold" />
                )}
              </motion.button>
            </div>
          </div>
          <div className="gothic-input w-full h-32 md:h-40 overflow-y-auto whitespace-pre-wrap">
            {outputText || (
              <span className="text-silver/50">Translation will appear here...</span>
            )}
          </div>
        </div>

        {/* Keyboard Shortcut Hint */}
        <div className="text-center text-xs text-silver/50 mt-4">
          Press <kbd className="px-2 py-1 bg-black-velvet border border-gold/20 rounded">Ctrl</kbd> +{' '}
          <kbd className="px-2 py-1 bg-black-velvet border border-gold/20 rounded">Enter</kbd> to
          translate
        </div>
      </motion.div>

      {/* Camera Modal */}
      <CameraModal
        isOpen={isCameraOpen}
        onClose={closeCamera}
        videoRef={videoRef}
        onCapture={captureImage}
      />
    </div>
  );
};

export default Translate;
