import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';

interface Task {
  id: number;
  description: string;
  priority: string;
  status: string;
  dueTime?: string;
  patientId?: number;
}

interface TaskCardProps {
  task: Task;
  onStatusChange?: (taskId: number, newStatus: string) => void;
}

export const TaskCard: React.FC<TaskCardProps> = ({ task, onStatusChange }) => {
  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'Critical': return '#ef4444';
      case 'High': return '#f97316';
      case 'Medium': return '#eab308';
      default: return '#6b7280';
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed': return '#10b981';
      case 'InProgress': return '#3b82f6';
      default: return '#6b7280';
    }
  };

  return (
    <View style={styles.card}>
      <View style={styles.header}>
        <View style={[styles.badge, { backgroundColor: getPriorityColor(task.priority) }]}>
          <Text style={styles.badgeText}>{task.priority}</Text>
        </View>
        <View style={[styles.badge, { backgroundColor: getStatusColor(task.status) }]}>
          <Text style={styles.badgeText}>{task.status}</Text>
        </View>
      </View>
      
      <Text style={styles.description}>{task.description}</Text>
      
      {task.dueTime && (
        <Text style={styles.dueTime}>
          Due: {new Date(task.dueTime).toLocaleString()}
        </Text>
      )}

      <View style={styles.actions}>
        {task.status === 'Pending' && onStatusChange && (
          <TouchableOpacity
            style={[styles.button, styles.startButton]}
            onPress={() => onStatusChange(task.id, 'InProgress')}
          >
            <Text style={styles.buttonText}>Start</Text>
          </TouchableOpacity>
        )}
        {task.status === 'InProgress' && onStatusChange && (
          <TouchableOpacity
            style={[styles.button, styles.completeButton]}
            onPress={() => onStatusChange(task.id, 'Completed')}
          >
            <Text style={styles.buttonText}>Complete</Text>
          </TouchableOpacity>
        )}
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  card: {
    backgroundColor: '#ffffff',
    borderRadius: 8,
    padding: 16,
    marginBottom: 12,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  header: {
    flexDirection: 'row',
    marginBottom: 8,
    gap: 8,
  },
  badge: {
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 12,
  },
  badgeText: {
    color: '#ffffff',
    fontSize: 10,
    fontWeight: '600',
  },
  description: {
    fontSize: 14,
    color: '#1f2937',
    marginBottom: 8,
  },
  dueTime: {
    fontSize: 12,
    color: '#6b7280',
    marginBottom: 12,
  },
  actions: {
    flexDirection: 'row',
    justifyContent: 'flex-end',
  },
  button: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 6,
  },
  startButton: {
    backgroundColor: '#3b82f6',
  },
  completeButton: {
    backgroundColor: '#10b981',
  },
  buttonText: {
    color: '#ffffff',
    fontSize: 12,
    fontWeight: '600',
  },
});

