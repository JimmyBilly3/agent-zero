import React from 'react';
import { motion } from 'framer-motion';
import useStore from '../store/useStore';

const tones = [
  { value: 'formal', label: '正式 Formal', icon: '🎩' },
  { value: 'casual', label: '隨意 Casual', icon: '😊' },
  { value: 'classical', label: '文言 Classical', icon: '📜' },
];

const ToneSelector = () => {
  const { settings, setSettings } = useStore();

  return (
    <div className="flex flex-wrap justify-center gap-2 md:gap-3">
      {tones.map((tone) => {
        const isActive = settings.tone === tone.value;

        return (
          <motion.button
            key={tone.value}
            onClick={() => setSettings({ tone: tone.value })}
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            className={`
              px-3 md:px-4 py-2 rounded-lg text-xs md:text-sm font-gothic border-2 transition-all duration-300
              ${isActive
                ? 'bg-gradient-to-r from-crimson to-gold/30 border-gold text-gold shadow-lg shadow-gold/20'
                : 'bg-black-velvet/50 border-gold/20 text-silver/70 hover:border-gold/50 hover:text-silver'
              }
            `}
          >
            <span className="mr-1 md:mr-2">{tone.icon}</span>
            <span>{tone.label}</span>
          </motion.button>
        );
      })}
    </div>
  );
};

export default ToneSelector;
