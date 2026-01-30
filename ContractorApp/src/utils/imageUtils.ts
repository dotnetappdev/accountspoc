import * as ImagePicker from 'expo-image-picker';
import * as FileSystem from 'expo-file-system';
import db from '../database/database';

export const requestCameraPermissions = async () => {
  const { status } = await ImagePicker.requestCameraPermissionsAsync();
  return status === 'granted';
};

export const requestMediaLibraryPermissions = async () => {
  const { status } = await ImagePicker.requestMediaLibraryPermissionsAsync();
  return status === 'granted';
};

export const takePhoto = async (): Promise<string | null> => {
  const hasPermission = await requestCameraPermissions();
  if (!hasPermission) {
    alert('Camera permission is required to take photos');
    return null;
  }

  const result = await ImagePicker.launchCameraAsync({
    mediaTypes: ['images'],
    allowsEditing: true,
    aspect: [4, 3],
    quality: 0.8,
  });

  if (!result.canceled && result.assets[0]) {
    return result.assets[0].uri;
  }

  return null;
};

export const pickImage = async (): Promise<string | null> => {
  const hasPermission = await requestMediaLibraryPermissions();
  if (!hasPermission) {
    alert('Media library permission is required to select photos');
    return null;
  }

  const result = await ImagePicker.launchImageLibraryAsync({
    mediaTypes: ['images'],
    allowsEditing: true,
    aspect: [4, 3],
    quality: 0.8,
  });

  if (!result.canceled && result.assets[0]) {
    return result.assets[0].uri;
  }

  return null;
};

export const saveWorkEvidenceImage = async (
  workOrderId: number,
  imageUri: string,
  description?: string
): Promise<number | null> => {
  try {
    const fileName = `work_evidence_${workOrderId}_${Date.now()}.jpg`;
    const directory = `${FileSystem.documentDirectory}work_evidence/`;
    
    // Create directory if it doesn't exist
    const dirInfo = await FileSystem.getInfoAsync(directory);
    if (!dirInfo.exists) {
      await FileSystem.makeDirectoryAsync(directory, { intermediates: true });
    }

    const newPath = `${directory}${fileName}`;
    await FileSystem.copyAsync({
      from: imageUri,
      to: newPath,
    });

    // Save to database
    const result = db.runSync(
      `INSERT INTO work_evidence_images (workOrderId, imagePath, description, capturedAt, syncStatus) 
       VALUES (?, ?, ?, ?, ?)`,
      [workOrderId, newPath, description || '', new Date().toISOString(), 'pending']
    );

    return result.lastInsertRowId;
  } catch (error) {
    console.error('Error saving work evidence image:', error);
    return null;
  }
};

export const getWorkEvidenceImages = (workOrderId: number) => {
  try {
    return db.getAllSync(
      'SELECT * FROM work_evidence_images WHERE workOrderId = ? ORDER BY capturedAt DESC',
      [workOrderId]
    ) as any[];
  } catch (error) {
    console.error('Error getting work evidence images:', error);
    return [];
  }
};

export const deleteWorkEvidenceImage = async (imageId: number, imagePath: string) => {
  try {
    // Delete file
    const fileInfo = await FileSystem.getInfoAsync(imagePath);
    if (fileInfo.exists) {
      await FileSystem.deleteAsync(imagePath);
    }

    // Delete from database
    db.runSync('DELETE FROM work_evidence_images WHERE id = ?', [imageId]);
    return true;
  } catch (error) {
    console.error('Error deleting work evidence image:', error);
    return false;
  }
};

export const saveSignature = async (signatureUri: string): Promise<string | null> => {
  try {
    const fileName = `signature_${Date.now()}.jpg`;
    const directory = `${FileSystem.documentDirectory}signatures/`;
    
    // Create directory if it doesn't exist
    const dirInfo = await FileSystem.getInfoAsync(directory);
    if (!dirInfo.exists) {
      await FileSystem.makeDirectoryAsync(directory, { intermediates: true });
    }

    const newPath = `${directory}${fileName}`;
    await FileSystem.copyAsync({
      from: imageUri,
      to: newPath,
    });

    return newPath;
  } catch (error) {
    console.error('Error saving signature:', error);
    return null;
  }
};
