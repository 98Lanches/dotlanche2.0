﻿using DotLanches.Domain.Entities;

namespace DotLanches.Domain.Interfaces.Gateways
{
    public interface IProdutoGateway
    {
        public Task Add(Produto produto);

        public Task<Produto> Edit(Produto produto);

        public Task<Produto> Delete(Guid idProduto);

        public Task<IEnumerable<Produto>> GetByCategoria(ECategoria categoria);

        public Task<Produto> GetById(Guid idProduto);
    }
}
