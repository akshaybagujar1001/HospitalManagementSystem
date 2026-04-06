import AsyncStorage from '@react-native-async-storage/async-storage';

const STORAGE_KEYS = {
  TASKS: 'nurse_tasks',
  PATIENTS: 'nurse_patients',
  LAST_SYNC: 'last_sync_timestamp',
  USER_TOKEN: 'user_token',
};

export const StorageService = {
  // Tasks
  async saveTasks(tasks: any[]): Promise<void> {
    try {
      await AsyncStorage.setItem(STORAGE_KEYS.TASKS, JSON.stringify(tasks));
    } catch (error) {
      console.error('Error saving tasks:', error);
      throw error;
    }
  },

  async getTasks(): Promise<any[]> {
    try {
      const data = await AsyncStorage.getItem(STORAGE_KEYS.TASKS);
      return data ? JSON.parse(data) : [];
    } catch (error) {
      console.error('Error getting tasks:', error);
      return [];
    }
  },

  // Patients
  async savePatients(patients: any[]): Promise<void> {
    try {
      await AsyncStorage.setItem(STORAGE_KEYS.PATIENTS, JSON.stringify(patients));
    } catch (error) {
      console.error('Error saving patients:', error);
      throw error;
    }
  },

  async getPatients(): Promise<any[]> {
    try {
      const data = await AsyncStorage.getItem(STORAGE_KEYS.PATIENTS);
      return data ? JSON.parse(data) : [];
    } catch (error) {
      console.error('Error getting patients:', error);
      return [];
    }
  },

  // Sync
  async saveSyncTimestamp(timestamp: number): Promise<void> {
    try {
      await AsyncStorage.setItem(STORAGE_KEYS.LAST_SYNC, timestamp.toString());
    } catch (error) {
      console.error('Error saving sync timestamp:', error);
    }
  },

  async getLastSyncTimestamp(): Promise<number | null> {
    try {
      const timestamp = await AsyncStorage.getItem(STORAGE_KEYS.LAST_SYNC);
      return timestamp ? parseInt(timestamp, 10) : null;
    } catch (error) {
      console.error('Error getting sync timestamp:', error);
      return null;
    }
  },

  // Auth
  async saveToken(token: string): Promise<void> {
    try {
      await AsyncStorage.setItem(STORAGE_KEYS.USER_TOKEN, token);
    } catch (error) {
      console.error('Error saving token:', error);
    }
  },

  async getToken(): Promise<string | null> {
    try {
      return await AsyncStorage.getItem(STORAGE_KEYS.USER_TOKEN);
    } catch (error) {
      console.error('Error getting token:', error);
      return null;
    }
  },

  async clearAll(): Promise<void> {
    try {
      await AsyncStorage.multiRemove(Object.values(STORAGE_KEYS));
    } catch (error) {
      console.error('Error clearing storage:', error);
    }
  },
};

