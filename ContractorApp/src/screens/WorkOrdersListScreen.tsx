import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

const WorkOrdersListScreen = () => {
  return (
    <View style={styles.container}>
      <Text style={styles.text}>Work Orders List Screen</Text>
      <Text style={styles.subtext}>Similar to Sales Orders - Coming Soon</Text>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#F2F2F7',
  },
  text: {
    fontSize: 20,
    fontWeight: '600',
    marginBottom: 10,
  },
  subtext: {
    fontSize: 14,
    color: '#8E8E93',
  },
});

export default WorkOrdersListScreen;
