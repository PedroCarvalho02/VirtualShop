import axios from "axios";

const baseURL = "http://localhost:5000/api/Product";

const productService = {
  getAllProducts: async () => {
    try {
      const response = await axios.get(baseURL);
      console.log("Resposta da API:", response.data); // Adicione este log para verificar a resposta
      // Acessar a propriedade $values para obter os produtos
      return response.data.$values || [];
    } catch (error) {
      console.error("Erro ao buscar produtos:", error);
      return [];
    }
  },
  addProduct: async (product) => {
    const response = await axios.post(baseURL, product);
    return response.data;
  },
  deleteProduct: async (id) => {
    const response = await axios.delete(`${baseURL}/${id}`);
    return response.data;
  },
};

export default productService;