import React from 'react';
import { motion } from 'framer-motion';
import { ArrowLeftRight } from 'lucide-react';
import useStore from '../store/useStore';

const languages = [
  { code: 'en', name: 'English', native: 'English' },
  { code: 'zh', name: 'Chinese', native: '中文' },
  { code: 'th', name: 'Thai', native: 'ไทย' },
];

const LanguageSelector = () => {
  const { settings, setSettings, swapLanguages } = useStore();

  return (
    <div className="flex items-center justify-center space-x-2 md:space-x-4">
      {/* Source Language */}
      <select
        value={settings.sourceLang}
        onChange={(e) => setSettings({ sourceLang: e.target.value })}
        className="gothic-input py-2 px-3 md:px-4 text-sm md:text-base cursor-pointer"
      >
        {languages.map((lang) => (
          <option key={lang.code} value={lang.code} className="bg-black-velvet">
            {lang.native}
          </option>
        ))}
      </select>

      {/* Swap Button */}
      <motion.button
        onClick={swapLanguages}
        whileHover={{ scale: 1.1, rotate: 180 }}
        whileTap={{ scale: 0.9 }}
        className="p-2 md:p-3 rounded-full bg-gradient-to-br from-crimson to-gold/50 text-gold hover:from-gold hover:to-crimson transition-all duration-300 border border-gold/30"
      >
        <ArrowLeftRight className="w-4 h-4 md:w-5 md:h-5" />
      </motion.button>

      {/* Target Language */}
      <select
        value={settings.targetLang}
        onChange={(e) => setSettings({ targetLang: e.target.value })}
        className="gothic-input py-2 px-3 md:px-4 text-sm md:text-base cursor-pointer"
      >
        {languages.map((lang) => (
          <option key={lang.code} value={lang.code} className="bg-black-velvet">
            {lang.native}
          </option>
        ))}
      </select>
    </div>
  );
};

export default LanguageSelector;
