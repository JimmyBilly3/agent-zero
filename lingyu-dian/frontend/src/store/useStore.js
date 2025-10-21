import { create } from 'zustand';
import { getSettings, saveSettings } from '../utils/storage';

const useStore = create((set, get) => ({
  // Settings state
  settings: {
    sourceLang: 'en',
    targetLang: 'zh',
    tone: 'casual',
    voiceGender: 'female',
    theme: 'dark',
    offlineMode: false,
    autoDetect: true,
  },

  // Translation state
  inputText: '',
  outputText: '',
  isTranslating: false,
  isOnline: navigator.onLine,

  // UI state
  activeTab: 'translate',
  showOverlay: false,

  // Actions
  setSettings: (newSettings) => {
    const updatedSettings = { ...get().settings, ...newSettings };
    set({ settings: updatedSettings });
    saveSettings(updatedSettings);
  },

  loadSettings: async () => {
    const savedSettings = await getSettings();
    if (savedSettings) {
      set({ settings: savedSettings });
    }
  },

  setInputText: (text) => set({ inputText: text }),
  setOutputText: (text) => set({ outputText: text }),
  setIsTranslating: (status) => set({ isTranslating: status }),
  setIsOnline: (status) => set({ isOnline: status }),
  setActiveTab: (tab) => set({ activeTab: tab }),
  setShowOverlay: (show) => set({ showOverlay: show }),

  // Swap languages
  swapLanguages: () => {
    const { settings, inputText, outputText } = get();
    set({
      settings: {
        ...settings,
        sourceLang: settings.targetLang,
        targetLang: settings.sourceLang,
      },
      inputText: outputText,
      outputText: inputText,
    });
    saveSettings({
      ...settings,
      sourceLang: settings.targetLang,
      targetLang: settings.sourceLang,
    });
  },
}));

export default useStore;
