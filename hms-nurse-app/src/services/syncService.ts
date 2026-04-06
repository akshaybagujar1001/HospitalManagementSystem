import { StorageService } from './storageService';
import { api } from './api';
import NetInfo from '@react-native-community/netinfo';

interface Task {
  id: number;
  status: string;
  updatedAt?: string;
  localUpdated?: boolean;
}

export const SyncService = {
  async syncTasks(): Promise<void> {
    try {
      const isConnected = await NetInfo.fetch().then(state => state.isConnected);
      
      if (!isConnected) {
        console.log('No internet connection. Working offline.');
        return;
      }

      // Get local tasks
      const localTasks = await StorageService.getTasks();
      
      // Get server tasks
      const serverTasks = await api.get('/nurse-tasks').then(res => res.data);
      
      // Merge tasks - server takes precedence, but keep local changes
      const merged = this.mergeTasks(localTasks, serverTasks);
      
      // Save merged tasks
      await StorageService.saveTasks(merged);
      
      // Upload pending local changes
      await this.uploadPendingChanges(localTasks);
      
      // Update sync timestamp
      await StorageService.saveSyncTimestamp(Date.now());
      
      console.log('Sync completed successfully');
    } catch (error) {
      console.error('Sync failed:', error);
      throw error;
    }
  },

  mergeTasks(local: Task[], server: Task[]): Task[] {
    const merged: Task[] = [];
    const serverTaskMap = new Map(server.map(t => [t.id, t]));
    
    // Add/update server tasks
    server.forEach(serverTask => {
      const localTask = local.find(t => t.id === serverTask.id);
      
      if (localTask && localTask.localUpdated) {
        // Keep local changes if they exist
        merged.push({
          ...serverTask,
          ...localTask,
          localUpdated: true, // Mark for upload
        });
      } else {
        merged.push(serverTask);
      }
    });
    
    // Add local-only tasks (if any)
    local.forEach(localTask => {
      if (!serverTaskMap.has(localTask.id)) {
        merged.push({ ...localTask, localUpdated: true });
      }
    });
    
    return merged;
  },

  async uploadPendingChanges(localTasks: Task[]): Promise<void> {
    const pendingTasks = localTasks.filter(t => t.localUpdated);
    
    for (const task of pendingTasks) {
      try {
        await api.put(`/nurse-tasks/${task.id}/status`, { status: task.status });
        // Remove localUpdated flag after successful upload
        task.localUpdated = false;
      } catch (error) {
        console.error(`Failed to upload task ${task.id}:`, error);
        // Keep localUpdated flag for retry
      }
    }
    
    // Save updated tasks
    await StorageService.saveTasks(localTasks);
  },

  async syncPatients(): Promise<void> {
    try {
      const isConnected = await NetInfo.fetch().then(state => state.isConnected);
      
      if (!isConnected) {
        return;
      }

      const patients = await api.get('/patients').then(res => res.data);
      await StorageService.savePatients(patients);
    } catch (error) {
      console.error('Failed to sync patients:', error);
    }
  },
};

