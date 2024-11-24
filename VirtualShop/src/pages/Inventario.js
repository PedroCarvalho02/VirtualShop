import { useState } from "react";
import ProductForm from "../components/Produtos/ProductForm";
import ProductList from "../components/Produtos/ProductList";

const Inventario = () => {
  const [refresh, setRefresh] = useState(false);

  const handleProductAdded = () => {
    setRefresh(!refresh); // Toggle refresh state to trigger re-render
  };

  return (
    <div>
      <ProductList key={refresh} />
      <ProductForm onProductAdded={handleProductAdded} />
    </div>
  );
};
export default Inventario;
