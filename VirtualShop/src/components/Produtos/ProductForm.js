import React, { useState } from "react";
import productService from "../../services/productService";

const ProductForm = ({ onProductAdded }) => {
  const [name, setName] = useState("");
  const [price, setPrice] = useState("");
  const [quantity, setQuantity] = useState("");
  const [imageUrl, setImageUrl] = useState("");

  const handleSubmit = async (event) => {
    event.preventDefault();
    const newProduct = { name, price: parseFloat(price), quantity, imageUrl };
    try {
      await productService.addProduct(newProduct);
      onProductAdded();
      setName("");
      setPrice("");
      setQuantity("");
      setImageUrl("");
    } catch (error) {
      console.error("Erro ao adicionar produto:", error);
    }
  };

  return (
    <div className="container">
      <h2 className="my-4">Adicionar Produto</h2>
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label htmlFor="productName" className="form-label">
            Nome:
          </label>
          <input
            type="text"
            className="form-control"
            id="productName"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="productPrice" className="form-label">
            Preço:
          </label>
          <input
            type="number"
            className="form-control"
            id="productPrice"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="productQuantity" className="form-label">
            Quantidade
          </label>
          <input
            type="number"
            className="form-control"
            id="productQuantity"
            value={quantity}
            onChange={(e) => setQuantity(e.target.value)}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="imageUrl" className="form-label">
            Imagem (URL):
          </label>
          <input
            type="text"
            className="form-control"
            id="imageUrl"
            value={imageUrl}
            onChange={(e) => setImageUrl(e.target.value)}
            required
          />
        </div>
        <button type="submit" className="btn btn-primary">
          Adicionar Produto
        </button>
      </form>
    </div>
  );
};

export default ProductForm;
