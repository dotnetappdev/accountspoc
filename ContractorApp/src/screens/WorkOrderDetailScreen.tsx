import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  ScrollView,
  StyleSheet,
  TouchableOpacity,
  Image,
  Alert,
  TextInput,
} from 'react-native';
import { useRoute, useNavigation } from '@react-navigation/native';
import { Ionicons } from '@expo/vector-icons';
import db from '../database/database';
import { takePhoto, pickImage, saveWorkEvidenceImage, getWorkEvidenceImages, deleteWorkEvidenceImage } from '../utils/imageUtils';
import { useTheme } from '../contexts/ThemeContext';

const WorkOrderDetailScreen = () => {
  const route = useRoute<any>();
  const navigation = useNavigation();
  const { colors } = useTheme();
  const workOrderId = route.params?.id;

  const [workOrder, setWorkOrder] = useState<any>(null);
  const [tasks, setTasks] = useState<any[]>([]);
  const [images, setImages] = useState<any[]>([]);
  const [newImageDescription, setNewImageDescription] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadWorkOrderDetails();
  }, [workOrderId]);

  const loadWorkOrderDetails = () => {
    try {
      const wo = db.getFirstSync('SELECT * FROM work_orders WHERE id = ?', [workOrderId]) as any;
      const woTasks = db.getAllSync('SELECT * FROM work_order_tasks WHERE workOrderId = ? ORDER BY sortOrder', [workOrderId]) as any[];
      const woImages = getWorkEvidenceImages(workOrderId);

      setWorkOrder(wo);
      setTasks(woTasks);
      setImages(woImages);
    } catch (error) {
      console.error('Error loading work order details:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleTakePhoto = async () => {
    const photoUri = await takePhoto();
    if (photoUri) {
      const imageId = await saveWorkEvidenceImage(workOrderId, photoUri, newImageDescription);
      if (imageId) {
        Alert.alert('Success', 'Photo saved successfully');
        setNewImageDescription('');
        loadWorkOrderDetails();
      } else {
        Alert.alert('Error', 'Failed to save photo');
      }
    }
  };

  const handlePickImage = async () => {
    const imageUri = await pickImage();
    if (imageUri) {
      const imageId = await saveWorkEvidenceImage(workOrderId, imageUri, newImageDescription);
      if (imageId) {
        Alert.alert('Success', 'Image saved successfully');
        setNewImageDescription('');
        loadWorkOrderDetails();
      } else {
        Alert.alert('Error', 'Failed to save image');
      }
    }
  };

  const handleDeleteImage = (imageId: number, imagePath: string) => {
    Alert.alert(
      'Delete Image',
      'Are you sure you want to delete this image?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Delete',
          style: 'destructive',
          onPress: async () => {
            const success = await deleteWorkEvidenceImage(imageId, imagePath);
            if (success) {
              loadWorkOrderDetails();
            } else {
              Alert.alert('Error', 'Failed to delete image');
            }
          },
        },
      ]
    );
  };

  const toggleTaskCompletion = (taskId: number, currentStatus: boolean) => {
    try {
      const newStatus = currentStatus ? 0 : 1;
      const completedDate = newStatus ? new Date().toISOString() : null;
      
      db.runSync(
        'UPDATE work_order_tasks SET isCompleted = ?, completedDate = ? WHERE id = ?',
        [newStatus, completedDate, taskId]
      );
      
      loadWorkOrderDetails();
    } catch (error) {
      console.error('Error toggling task:', error);
      Alert.alert('Error', 'Failed to update task');
    }
  };

  const completeWorkOrder = () => {
    Alert.alert(
      'Complete Work Order',
      'Mark this work order as completed?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Complete',
          onPress: () => {
            try {
              db.runSync(
                'UPDATE work_orders SET status = ?, completedDate = ?, updatedAt = ? WHERE id = ?',
                ['Completed', new Date().toISOString(), new Date().toISOString(), workOrderId]
              );
              Alert.alert('Success', 'Work order marked as completed');
              navigation.goBack();
            } catch (error) {
              console.error('Error completing work order:', error);
              Alert.alert('Error', 'Failed to complete work order');
            }
          },
        },
      ]
    );
  };

  if (loading || !workOrder) {
    return (
      <View style={[styles.container, { backgroundColor: colors.background }]}>
        <Text style={[styles.loadingText, { color: colors.text }]}>Loading...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={[styles.container, { backgroundColor: colors.background }]}>
      <View style={[styles.card, { backgroundColor: colors.card, borderColor: colors.border }]}>
        <Text style={[styles.title, { color: colors.text }]}>{workOrder.workOrderNumber}</Text>
        <Text style={[styles.customer, { color: colors.text }]}>{workOrder.customerName}</Text>
        <Text style={[styles.description, { color: colors.textSecondary }]}>{workOrder.description}</Text>
        
        <View style={styles.infoRow}>
          <Ionicons name="calendar" size={16} color={colors.textSecondary} />
          <Text style={[styles.infoText, { color: colors.textSecondary }]}>
            Scheduled: {new Date(workOrder.scheduledDate).toLocaleDateString()}
          </Text>
        </View>

        <View style={styles.statusRow}>
          <View style={[styles.badge, { backgroundColor: colors.primary }]}>
            <Text style={styles.badgeText}>{workOrder.status}</Text>
          </View>
          <View style={[styles.badge, { backgroundColor: colors.warning }]}>
            <Text style={styles.badgeText}>{workOrder.priority}</Text>
          </View>
        </View>
      </View>

      <View style={[styles.card, { backgroundColor: colors.card, borderColor: colors.border }]}>
        <Text style={[styles.sectionTitle, { color: colors.text }]}>Tasks</Text>
        {tasks.map((task) => (
          <TouchableOpacity
            key={task.id}
            style={styles.taskItem}
            onPress={() => toggleTaskCompletion(task.id, task.isCompleted === 1)}
          >
            <Ionicons
              name={task.isCompleted ? 'checkbox' : 'square-outline'}
              size={24}
              color={task.isCompleted ? colors.success : colors.textSecondary}
            />
            <Text
              style={[
                styles.taskText,
                { color: colors.text },
                task.isCompleted && styles.taskCompleted,
              ]}
            >
              {task.taskName}
            </Text>
          </TouchableOpacity>
        ))}
      </View>

      <View style={[styles.card, { backgroundColor: colors.card, borderColor: colors.border }]}>
        <Text style={[styles.sectionTitle, { color: colors.text }]}>Work Evidence</Text>
        
        <TextInput
          style={[styles.input, { backgroundColor: colors.background, color: colors.text, borderColor: colors.border }]}
          value={newImageDescription}
          onChangeText={setNewImageDescription}
          placeholder="Image description (optional)"
          placeholderTextColor={colors.textSecondary}
        />

        <View style={styles.imageButtons}>
          <TouchableOpacity
            style={[styles.imageButton, { backgroundColor: colors.primary }]}
            onPress={handleTakePhoto}
          >
            <Ionicons name="camera" size={20} color="#fff" />
            <Text style={styles.imageButtonText}>Take Photo</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={[styles.imageButton, { backgroundColor: colors.info }]}
            onPress={handlePickImage}
          >
            <Ionicons name="images" size={20} color="#fff" />
            <Text style={styles.imageButtonText}>Choose Image</Text>
          </TouchableOpacity>
        </View>

        {images.length > 0 && (
          <View style={styles.imageGallery}>
            {images.map((img) => (
              <View key={img.id} style={styles.imageContainer}>
                <Image source={{ uri: img.imagePath }} style={styles.image} />
                {img.description && (
                  <Text style={[styles.imageDescription, { color: colors.textSecondary }]}>
                    {img.description}
                  </Text>
                )}
                <TouchableOpacity
                  style={[styles.deleteImageButton, { backgroundColor: colors.danger }]}
                  onPress={() => handleDeleteImage(img.id, img.imagePath)}
                >
                  <Ionicons name="trash" size={16} color="#fff" />
                </TouchableOpacity>
              </View>
            ))}
          </View>
        )}
      </View>

      {workOrder.status !== 'Completed' && (
        <TouchableOpacity
          style={[styles.completeButton, { backgroundColor: colors.success }]}
          onPress={completeWorkOrder}
        >
          <Ionicons name="checkmark-circle" size={20} color="#fff" />
          <Text style={styles.completeButtonText}>Complete Work Order</Text>
        </TouchableOpacity>
      )}
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  loadingText: {
    textAlign: 'center',
    marginTop: 50,
    fontSize: 16,
  },
  card: {
    margin: 15,
    padding: 15,
    borderRadius: 12,
    borderWidth: 1,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 8,
  },
  customer: {
    fontSize: 18,
    marginBottom: 8,
  },
  description: {
    fontSize: 14,
    marginBottom: 12,
  },
  infoRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 12,
  },
  infoText: {
    marginLeft: 8,
    fontSize: 14,
  },
  statusRow: {
    flexDirection: 'row',
    gap: 10,
  },
  badge: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 12,
  },
  badgeText: {
    color: '#fff',
    fontSize: 12,
    fontWeight: '600',
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: '600',
    marginBottom: 12,
  },
  taskItem: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
  },
  taskText: {
    marginLeft: 12,
    fontSize: 16,
    flex: 1,
  },
  taskCompleted: {
    textDecorationLine: 'line-through',
    opacity: 0.6,
  },
  input: {
    borderWidth: 1,
    borderRadius: 8,
    padding: 12,
    marginBottom: 12,
    fontSize: 14,
  },
  imageButtons: {
    flexDirection: 'row',
    gap: 10,
    marginBottom: 15,
  },
  imageButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 12,
    borderRadius: 8,
    gap: 8,
  },
  imageButtonText: {
    color: '#fff',
    fontSize: 14,
    fontWeight: '600',
  },
  imageGallery: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: 10,
  },
  imageContainer: {
    width: '48%',
    position: 'relative',
  },
  image: {
    width: '100%',
    height: 150,
    borderRadius: 8,
  },
  imageDescription: {
    fontSize: 12,
    marginTop: 4,
  },
  deleteImageButton: {
    position: 'absolute',
    top: 8,
    right: 8,
    width: 32,
    height: 32,
    borderRadius: 16,
    justifyContent: 'center',
    alignItems: 'center',
  },
  completeButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    margin: 15,
    padding: 15,
    borderRadius: 8,
    gap: 8,
  },
  completeButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
});

export default WorkOrderDetailScreen;
