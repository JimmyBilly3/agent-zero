import React, { useState } from 'react';
import { motion } from 'framer-motion';
import { Save, RotateCcw, Wifi, WifiOff, Volume2, Moon, Sun } from 'lucide-react';
import useStore from '../store/useStore';

const Settings = () => {
  const { settings, setSettings } = useStore();
  const [saved, setSaved] = useState(false);

  const handleSave = () => {
    setSaved(true);
    setTimeout(() => setSaved(false), 2000);
  };

  const handleReset = () => {
    if (window.confirm('Are you sure you want to reset all settings to default?')) {
      setSettings({
        sourceLang: 'en',
        targetLang: 'zh',
        tone: 'casual',
        voiceGender: 'female',
        theme: 'dark',
        offlineMode: false,
        autoDetect: true,
      });
      handleSave();
    }
  };

  const toggleSetting = (key) => {
    setSettings({ [key]: !settings[key] });
  };

  return (
    <div className="max-w-4xl mx-auto">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="gothic-card p-6 md:p-8"
      >
        {/* Header */}
        <h2 className="text-2xl md:text-3xl font-chinese text-gold glow-text mb-6">
          設定 Settings
        </h2>

        {/* Language Settings */}
        <section className="mb-8">
          <h3 className="text-lg font-gothic text-gold mb-4 flex items-center">
            <span className="mr-2">🌍</span>
            Language Settings
          </h3>
          <div className="space-y-4">
            <div>
              <label className="block text-silver text-sm mb-2">Default Source Language</label>
              <select
                value={settings.sourceLang}
                onChange={(e) => setSettings({ sourceLang: e.target.value })}
                className="gothic-input w-full"
              >
                <option value="en">English</option>
                <option value="zh">Chinese (中文)</option>
                <option value="th">Thai (ไทย)</option>
              </select>
            </div>

            <div>
              <label className="block text-silver text-sm mb-2">Default Target Language</label>
              <select
                value={settings.targetLang}
                onChange={(e) => setSettings({ targetLang: e.target.value })}
                className="gothic-input w-full"
              >
                <option value="en">English</option>
                <option value="zh">Chinese (中文)</option>
                <option value="th">Thai (ไทย)</option>
              </select>
            </div>

            <div className="flex items-center justify-between p-4 bg-black-velvet/50 border border-gold/20 rounded-lg">
              <div>
                <div className="text-silver text-sm font-gothic">Auto-detect Language</div>
                <div className="text-xs text-silver/60 mt-1">Automatically detect source language</div>
              </div>
              <motion.button
                onClick={() => toggleSetting('autoDetect')}
                whileTap={{ scale: 0.95 }}
                className={`relative w-14 h-8 rounded-full transition-all ${
                  settings.autoDetect ? 'bg-gold' : 'bg-gray-600'
                }`}
              >
                <motion.div
                  animate={{ x: settings.autoDetect ? 24 : 2 }}
                  className="absolute top-1 w-6 h-6 bg-black-velvet rounded-full shadow-md"
                />
              </motion.button>
            </div>
          </div>
        </section>

        {/* Voice Settings */}
        <section className="mb-8">
          <h3 className="text-lg font-gothic text-gold mb-4 flex items-center">
            <Volume2 className="w-5 h-5 mr-2" />
            Voice Settings
          </h3>
          <div className="space-y-4">
            <div>
              <label className="block text-silver text-sm mb-2">Voice Gender</label>
              <div className="grid grid-cols-2 gap-3">
                {['female', 'male'].map((gender) => (
                  <motion.button
                    key={gender}
                    onClick={() => setSettings({ voiceGender: gender })}
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                    className={`p-3 rounded-lg border-2 transition-all ${
                      settings.voiceGender === gender
                        ? 'bg-gradient-to-r from-crimson to-gold/30 border-gold text-gold'
                        : 'bg-black-velvet/50 border-gold/20 text-silver/70 hover:border-gold/50'
                    }`}
                  >
                    {gender === 'female' ? '👩 Female' : '👨 Male'}
                  </motion.button>
                ))}
              </div>
            </div>

            <div>
              <label className="block text-silver text-sm mb-2">Default Tone</label>
              <select
                value={settings.tone}
                onChange={(e) => setSettings({ tone: e.target.value })}
                className="gothic-input w-full"
              >
                <option value="formal">🎩 Formal (正式)</option>
                <option value="casual">😊 Casual (隨意)</option>
                <option value="classical">📜 Classical (文言)</option>
              </select>
            </div>
          </div>
        </section>

        {/* Connection Settings */}
        <section className="mb-8">
          <h3 className="text-lg font-gothic text-gold mb-4 flex items-center">
            <Wifi className="w-5 h-5 mr-2" />
            Connection Settings
          </h3>
          <div className="space-y-4">
            <div className="flex items-center justify-between p-4 bg-black-velvet/50 border border-gold/20 rounded-lg">
              <div>
                <div className="text-silver text-sm font-gothic flex items-center">
                  {settings.offlineMode ? (
                    <WifiOff className="w-4 h-4 mr-2 text-red-500" />
                  ) : (
                    <Wifi className="w-4 h-4 mr-2 text-green-500" />
                  )}
                  Offline Mode
                </div>
                <div className="text-xs text-silver/60 mt-1">
                  Use local models for translation
                </div>
              </div>
              <motion.button
                onClick={() => toggleSetting('offlineMode')}
                whileTap={{ scale: 0.95 }}
                className={`relative w-14 h-8 rounded-full transition-all ${
                  settings.offlineMode ? 'bg-crimson' : 'bg-gold'
                }`}
              >
                <motion.div
                  animate={{ x: settings.offlineMode ? 24 : 2 }}
                  className="absolute top-1 w-6 h-6 bg-black-velvet rounded-full shadow-md"
                />
              </motion.button>
            </div>
          </div>
        </section>

        {/* Theme Settings */}
        <section className="mb-8">
          <h3 className="text-lg font-gothic text-gold mb-4 flex items-center">
            <Moon className="w-5 h-5 mr-2" />
            Theme Settings
          </h3>
          <div className="p-4 bg-black-velvet/50 border border-gold/20 rounded-lg">
            <div className="text-silver text-sm">
              Current theme: <span className="text-gold font-gothic">Gothic Chinese Dark</span>
            </div>
            <div className="text-xs text-silver/60 mt-2">
              More themes coming soon...
            </div>
          </div>
        </section>

        {/* Action Buttons */}
        <div className="flex flex-col md:flex-row gap-3">
          <motion.button
            onClick={handleSave}
            whileHover={{ scale: 1.02 }}
            whileTap={{ scale: 0.98 }}
            className="gothic-button flex-1 flex items-center justify-center space-x-2"
          >
            <Save className="w-5 h-5" />
            <span>{saved ? '✓ Saved!' : 'Save Settings'}</span>
          </motion.button>

          <motion.button
            onClick={handleReset}
            whileHover={{ scale: 1.02 }}
            whileTap={{ scale: 0.98 }}
            className="flex-1 px-6 py-3 bg-black-velvet border-2 border-gold/30 rounded-lg text-silver hover:border-crimson hover:text-crimson transition-all flex items-center justify-center space-x-2"
          >
            <RotateCcw className="w-5 h-5" />
            <span>Reset to Default</span>
          </motion.button>
        </div>

        {/* Info */}
        <div className="mt-8 p-4 bg-gradient-to-r from-crimson/10 to-gold/10 border border-gold/20 rounded-lg">
          <div className="text-xs text-silver/70 space-y-1">
            <p>💡 <strong>Tip:</strong> Settings are saved automatically to your browser.</p>
            <p>🔒 <strong>Privacy:</strong> All data is stored locally on your device.</p>
            <p>⚡ <strong>Offline Mode:</strong> Download models for offline translation.</p>
          </div>
        </div>
      </motion.div>
    </div>
  );
};

export default Settings;
