﻿using DotLanches.Presenters.Dtos;
using DotLanches.Controllers;
using DotLanches.Domain.Interfaces.ExternalInterfaces;
using DotLanches.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using DotLanches.Api.Dtos;

namespace DotLanches.Api.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICheckout _checkout;

        public PagamentoController(IPagamentoRepository pagamentoRepository, IPedidoRepository pedidoRepository, ICheckout checkout)
        {
            _pagamentoRepository = pagamentoRepository;
            _pedidoRepository = pedidoRepository;
            _checkout = checkout;
        }

        /// <summary>
        /// Envia os dados para o meio de pagamento e retorna o QR Code para o cliente.
        /// </summary>
        /// <param name="pagamentoRequest">Dados de requisição do pagamento</param>
        /// <returns>QR Code para efetuar pagamento.</returns>
        [HttpPost("QrCode")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RequestPagamentoQrCode([Required][FromBody] PagamentoRequestDto pagamentoRequest)
        {
            var adapterPagamento = new AdapterPagamentoController(_pagamentoRepository, _pedidoRepository, _checkout);
            var qrCode = await adapterPagamento.RequestPagamentoQRCode(pagamentoRequest.IdPedido);
            return Ok(qrCode);
        }

        /// <summary>
        /// Recebe o status do pagamento e segue com a confirmação ou cancelamento do pedido.
        /// </summary>
        /// <param name="pagamentoResponse">Dados de requisição do pagamento</param>
        /// <returns>Situação do pagamento e senha para retirada do pedido.</returns>
        [HttpPost("Confirmar")]
        [ProducesResponseType(typeof(PagamentoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ProcessPagamento([Required][FromBody] ProcessPagamentoRequestDto pagamentoResponse)
        {
            var adapterPagamento = new AdapterPagamentoController(_pagamentoRepository, _pedidoRepository, _checkout);
            var queueKey = pagamentoResponse.IsAccepted ? await _pedidoRepository.AssignKey(pagamentoResponse.IdPedido) : 0;
            var payResponse = await adapterPagamento.ProcessPagamento(pagamentoResponse.IdPedido, pagamentoResponse.IsAccepted, queueKey);
            return Ok(payResponse);
        }
    }
}