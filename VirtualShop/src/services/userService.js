import axios from "axios";

const baseURL = "http://localhost:5000/api/User";

const userService = {
  register: async (userData) => {
    const response = await axios.post(`${baseURL}/register`, userData);
    return response.data;
  },
  login: async (loginData) => {
    const response = await axios.post(`${baseURL}/login`, loginData);
    return response.data;
  },
  logout: async (token) => {
    const response = await axios.post(`${baseURL}/logout`, null, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  },
  getProfile: async (token) => {
    const response = await axios.get(`${baseURL}/profile`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  },
};

export default userService;