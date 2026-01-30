import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

const QuoteFormScreen = () => {
  return (
    <View style={styles.container}>
      <Text style={styles.text}>Quote Form Screen</Text>
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
  },
});

export default QuoteFormScreen;
