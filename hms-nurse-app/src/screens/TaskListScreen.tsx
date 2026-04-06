import React, { useEffect, useState } from 'react';
import { View, FlatList, RefreshControl, Text, StyleSheet } from 'react-native';
import { useTasks } from '../hooks/useTasks';
import { TaskCard } from '../components/TaskCard';
import { SyncService } from '../services/syncService';

export const TaskListScreen: React.FC = () => {
  const { tasks, loading, refresh } = useTasks();
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    refresh();
    // Auto-sync when screen loads
    SyncService.syncTasks().catch(err => console.error('Auto-sync failed:', err));
  }, []);

  const onRefresh = async () => {
    setRefreshing(true);
    try {
      await SyncService.syncTasks();
      await refresh();
    } catch (error) {
      console.error('Refresh failed:', error);
    } finally {
      setRefreshing(false);
    }
  };

  if (loading && tasks.length === 0) {
    return (
      <View style={styles.container}>
        <Text>Loading tasks...</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <FlatList
        data={tasks}
        renderItem={({ item }) => <TaskCard task={item} />}
        keyExtractor={item => item.id.toString()}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
        }
        ListEmptyComponent={
          <View style={styles.emptyContainer}>
            <Text style={styles.emptyText}>No tasks assigned</Text>
          </View>
        }
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
    padding: 16,
  },
  emptyContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    paddingTop: 100,
  },
  emptyText: {
    fontSize: 16,
    color: '#666',
  },
});

