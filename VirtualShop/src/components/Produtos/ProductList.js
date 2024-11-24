import React, { useState, useEffect } from "react";
import ProductListItem from "./ProductListItem";
import productService from "../../services/productServices";

const ProductList = () => {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    try {
      const productsData = await productService.getAllProducts();
      setProducts(productsData);
    } catch (error) {
      console.error("Erro ao buscar produtos:", error);
    }
  };
  const handleDelete = async (id) => {
    try {
      await productService.deleteProduct(id);
      fetchProducts();
    } catch (error) {
      console.error("Erro ao deletar produto:", error);
    }
  };
  const handleEdit = () => {
    fetchProducts();
  };

  return (
    <div className="container">
      <h2 className="my-4">Lista de Produtos</h2>
      <ul className="list-group">
        {products.map((product) => (
          <ProductListItem
            key={product.id}
            product={product}
            onDelete={() => handleDelete(product.id)}
            onEdit={handleEdit}
          />
        ))}
      </ul>
    </div>
  );
};
export default ProductList;
