import axios from "axios";

const baseURL = "http://localhost:5000/api/Product";

const getToken = () => {
  return localStorage.getItem("token");
};

const productService = {
  getAllProducts: async () => {
    try {
      const response = await axios.get(baseURL, {
        headers: {
          'Authorization': `Bearer ${getToken()}`,
        },
      });
      const productsData = response.data.$values || [];
      return productsData;
    } catch (error) {
      console.error("Erro ao buscar produtos:", error);
      return [];
    }
  },
  addProduct: async (product) => {
    try {
      const response = await axios.post(baseURL, product, {
        headers: {
          'Authorization': `Bearer ${getToken()}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error("Erro ao adicionar produto:", error);
      throw error;
    }
  },
  updateProduct: async (id, product) => {
    try {
      const response = await axios.put(`${baseURL}/${id}`, product, {
        headers: {
          'Authorization': `Bearer ${getToken()}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error("Erro ao atualizar produto:", error);
      throw error;
    }
  },
  deleteProduct: async (id) => {
    try {
      const response = await axios.delete(`${baseURL}/${id}`, {
        headers: {
          'Authorization': `Bearer ${getToken()}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error("Erro ao deletar produto:", error);
      throw error;
    }
  },
};

export default productService;