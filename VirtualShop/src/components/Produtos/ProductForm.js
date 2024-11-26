import React, { useState, useEffect } from "react";
import productService from "../../services/productService";

const ProductForm = ({ onProductAdded, productToEdit }) => {
  const [name, setName] = useState("");
  const [price, setPrice] = useState("");
  const [imageUrl, setImageUrl] = useState("");

  useEffect(() => {
    if (productToEdit) {
      setName(productToEdit.nome);
      setPrice(productToEdit.preco);
      setImageUrl(productToEdit.imageUrl);
    }
  }, [productToEdit]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    const newProduct = { nome: name, preco: parseFloat(price), imageUrl };

    try {
      if (productToEdit) {
        await productService.updateProduct(productToEdit.id, newProduct);
      } else {
        await productService.addProduct(newProduct);
      }
      onProductAdded();
      setName("");
      setPrice("");
      setImageUrl("");
    } catch (error) {
      console.error("Erro ao adicionar/atualizar produto:", error);
    }
  };

  return (
    <div className="container">
      <h2 className="my-4">{productToEdit ? "Editar Produto" : "Adicionar Produto"}</h2>
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
          {productToEdit ? "Atualizar Produto" : "Adicionar Produto"}
        </button>
      </form>
    </div>
  );
};

export default ProductForm;