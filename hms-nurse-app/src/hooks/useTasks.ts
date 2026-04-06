import { useState, useEffect } from 'react';
import { StorageService } from '../services/storageService';
import { api } from '../services/api';
import NetInfo from '@react-native-community/netinfo';

interface Task {
  id: number;
  description: string;
  priority: string;
  status: string;
  dueTime?: string;
  patientId?: number;
  localUpdated?: boolean;
}

export const useTasks = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);

  const loadTasks = async () => {
    try {
      setLoading(true);
      
      // Always load from local storage first (offline-first)
      const localTasks = await StorageService.getTasks();
      setTasks(localTasks);
      
      // Try to fetch from server if online
      const isConnected = await NetInfo.fetch().then(state => state.isConnected);
      if (isConnected) {
        try {
          const response = await api.get('/nurse-tasks');
          const serverTasks = response.data;
          
          // Merge with local tasks
          const merged = mergeTasks(localTasks, serverTasks);
          setTasks(merged);
          await StorageService.saveTasks(merged);
        } catch (error) {
          console.error('Failed to fetch tasks from server:', error);
          // Continue with local tasks
        }
      }
    } catch (error) {
      console.error('Error loading tasks:', error);
    } finally {
      setLoading(false);
    }
  };

  const mergeTasks = (local: Task[], server: Task[]): Task[] => {
    const merged: Task[] = [];
    const serverTaskMap = new Map(server.map(t => [t.id, t]));
    
    server.forEach(serverTask => {
      const localTask = local.find(t => t.id === serverTask.id);
      if (localTask && localTask.localUpdated) {
        merged.push({ ...serverTask, ...localTask, localUpdated: true });
      } else {
        merged.push(serverTask);
      }
    });
    
    local.forEach(localTask => {
      if (!serverTaskMap.has(localTask.id)) {
        merged.push({ ...localTask, localUpdated: true });
      }
    });
    
    return merged;
  };

  const updateTaskStatus = async (taskId: number, newStatus: string) => {
    // Update locally first
    const updatedTasks = tasks.map(task =>
      task.id === taskId
        ? { ...task, status: newStatus, localUpdated: true }
        : task
    );
    setTasks(updatedTasks);
    await StorageService.saveTasks(updatedTasks);
    
    // Try to sync with server
    const isConnected = await NetInfo.fetch().then(state => state.isConnected);
    if (isConnected) {
      try {
        await api.put(`/nurse-tasks/${taskId}/status`, { status: newStatus });
        // Remove localUpdated flag after successful sync
        const syncedTasks = updatedTasks.map(task =>
          task.id === taskId ? { ...task, localUpdated: false } : task
        );
        setTasks(syncedTasks);
        await StorageService.saveTasks(syncedTasks);
      } catch (error) {
        console.error('Failed to sync task status:', error);
        // Keep localUpdated flag for later sync
      }
    }
  };

  useEffect(() => {
    loadTasks();
  }, []);

  return {
    tasks,
    loading,
    refresh: loadTasks,
    updateTaskStatus,
  };
};

