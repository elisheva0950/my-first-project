import axios from 'axios';

axios.defaults.baseURL = "http://localhost:5218";

// Interceptor לתפיסת שגיאות
axios.interceptors.response.use(
  response => response,
  error => {
    console.error('שגיאה:', error.response?.status, error.message);
    return Promise.reject(error);
  }
);

// eslint-disable-next-line
export default {
  getTasks: async () => {
    const result = await axios.get('/items');
    return result.data;
  },

  addTask: async (name) => {
    const result = await axios.post('/items', { name: name, isComplete: false });
    return result.data;
  },

  setCompleted: async (id, isComplete) => {
    await axios.put(`/items/${id}`, { id: id, isComplete: isComplete });
  },

  deleteTask: async (id) => {
    await axios.delete(`/items/${id}`);
  }
};