import React, { useState } from "react";
import productService from "../../services/productServices";

const ProductListItem = ({ product, onDelete, onEdit }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editedName, setEditedName] = useState(product.name);
  const [editedPrice, setEditedPrice] = useState(product.price);
  const [editedQuantity, setEditedQuantity] = useState(product.quantity);
  const [editedImage, setEditedImage] = useState(product.urlImage);

  const handleEdit = async () => {
    setIsEditing(true);
  };

  const handleSave = async () => {
    const editedProduct = {
      ...product,
      name: editedName,
      price: parseFloat(editedPrice),
      quantity: editedQuantity,
      imageUrl: editedImage,
    };

    try {
      await productService.updateProduct(product.id, editedProduct);
      setIsEditing(false);
      onEdit();
    } catch (error) {
      console.error("Erro ao atualizar produto:", error);
    }
  };
  const handleCancel = () => {
    setIsEditing(false);
    setEditedName(product.name);
    setEditedPrice(product.price);
    setEditedQuantity(product.quantity);
    setEditedImage(product.imageUrl);
  };
  return (
    <li className="list-group-item">
      {isEditing ? (
        <div className="row">
          <div className="col">
            <input
              type="text"
              className="form-control"
              value={editedName}
              onChange={(e) => setEditedName(e.target.value)}
              required
            />
          </div>
          <div className="col">
            <input
              type="number"
              className="form-control"
              value={editedPrice}
              onChange={(e) => setEditedPrice(e.target.value)}
              required
            />
          </div>
          <div className="col">
            <input
              type="number"
              className="form-control"
              value={editedQuantity}
              onChange={(e) => setEditedQuantity(e.target.value)}
              required
            />
          </div>
          <div className="col">
            <input
              type="text"
              className="form-control"
              value={editedImage}
              onChange={(e) => setEditedImage(e.target.value)}
              required
            />
          </div>
          <div className="col-auto">
            <button className="btn btn-success me-2" onClick={handleSave}>
              Salvar
            </button>
            <button className="btn btn-secondary" onClick={handleCancel}>
              Cancelar
            </button>
          </div>
        </div>
      ) : (
        <div className="d-flex justify-content-between align-items-center">
          <span>
            {product.name} - ${product.price}
          </span>
          <div>
            <button className="btn btn-danger me-2" onClick={onDelete}>
              Deletar
            </button>
            <button className="btn btn-primary" onClick={handleEdit}>
              Editar
            </button>
          </div>
        </div>
      )}
    </li>
  );
};
export default ProductListItem;
