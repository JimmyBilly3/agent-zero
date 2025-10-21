import localforage from 'localforage';

// Configure localforage
localforage.config({
  name: 'LingYuDian',
  storeName: 'translations',
  description: 'Translation history and settings storage',
});

// History storage
export const saveTranslation = async (translation) => {
  try {
    const history = await getTranslationHistory();
    const newTranslation = {
      id: Date.now().toString(),
      timestamp: new Date().toISOString(),
      ...translation,
    };
    history.unshift(newTranslation);

    // Keep only last 100 translations
    const trimmedHistory = history.slice(0, 100);
    await localforage.setItem('history', trimmedHistory);
    return newTranslation;
  } catch (error) {
    console.error('Error saving translation:', error);
    throw error;
  }
};

export const getTranslationHistory = async () => {
  try {
    const history = await localforage.getItem('history');
    return history || [];
  } catch (error) {
    console.error('Error getting translation history:', error);
    return [];
  }
};

export const clearTranslationHistory = async () => {
  try {
    await localforage.setItem('history', []);
  } catch (error) {
    console.error('Error clearing translation history:', error);
    throw error;
  }
};

export const deleteTranslation = async (id) => {
  try {
    const history = await getTranslationHistory();
    const updatedHistory = history.filter(item => item.id !== id);
    await localforage.setItem('history', updatedHistory);
  } catch (error) {
    console.error('Error deleting translation:', error);
    throw error;
  }
};

// Settings storage
export const saveSettings = async (settings) => {
  try {
    await localforage.setItem('settings', settings);
  } catch (error) {
    console.error('Error saving settings:', error);
    throw error;
  }
};

export const getSettings = async () => {
  try {
    const settings = await localforage.getItem('settings');
    return settings || {
      sourceLang: 'en',
      targetLang: 'zh',
      tone: 'casual',
      voiceGender: 'female',
      theme: 'dark',
      offlineMode: false,
      autoDetect: true,
    };
  } catch (error) {
    console.error('Error getting settings:', error);
    return null;
  }
};
