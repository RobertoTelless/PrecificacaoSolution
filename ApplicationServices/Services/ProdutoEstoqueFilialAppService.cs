using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class ProdutoEstoqueFilialAppService : AppServiceBase<PRODUTO_ESTOQUE_EMPRESA>, IProdutoEstoqueFilialAppService
    {
        private readonly IProdutoEstoqueFilialService _baseService;
        private readonly IMovimentoEstoqueProdutoService _movService;

        public ProdutoEstoqueFilialAppService(IProdutoEstoqueFilialService baseService, IMovimentoEstoqueProdutoService movService) : base(baseService)
        {
            _baseService = baseService;
            _movService = movService;
        }

        public List<PRODUTO_ESTOQUE_EMPRESA> GetAllItens(Int32 idAss)
        {
            return _baseService.GetAllItens(idAss);
        }

        public PRODUTO_ESTOQUE_EMPRESA CheckExist(PRODUTO_ESTOQUE_EMPRESA conta, Int32 idAss)
        {
            PRODUTO_ESTOQUE_EMPRESA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public PRODUTO_ESTOQUE_EMPRESA GetItemById(Int32 id)
        {
            PRODUTO_ESTOQUE_EMPRESA item = _baseService.GetItemById(id);
            return item;
        }

        public PRODUTO_ESTOQUE_EMPRESA GetItemById(PRODUTO item)
        {
            PRODUTO_ESTOQUE_EMPRESA obj = _baseService.GetItemById(item);
            return obj;
        }

        public List<PRODUTO_ESTOQUE_EMPRESA> GetByProd(Int32 id)
        {
            return _baseService.GetByProd(id);
        }

        public PRODUTO_ESTOQUE_EMPRESA GetByProdFilial(Int32 prod, Int32 fili)
        {
            PRODUTO_ESTOQUE_EMPRESA item = _baseService.GetByProdFilial(prod, fili);
            return item;
        }

        public Int32 ValidateCreate(PRODUTO_ESTOQUE_EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto

                //Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PRODUTO_ESTOQUE_EMPRESA item, PRODUTO_ESTOQUE_EMPRESA itemAntes, USUARIO usuario)
        {
            try
            {
                if (item.EMPRESA != null)
                {
                    item.EMPRESA = null;
                }

                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditEstoque(PRODUTO_ESTOQUE_EMPRESA item, PRODUTO_ESTOQUE_EMPRESA itemAntes, USUARIO usuario)
        {
            try
            {
                if (item.EMPRESA != null)
                {
                    item.EMPRESA = null;
                }

                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }

                Int32 operacao = item.PREE_QN_ESTOQUE < item.PREF_QN_QUANTIDADE_ALTERADA ? 1 : 2;
                Int32 quant = 0;
                Int32 tipo = 0;
                if (item.PREF_QN_QUANTIDADE_ALTERADA > item.PREE_QN_ESTOQUE)
                {
                    quant = (Int32)item.PREF_QN_QUANTIDADE_ALTERADA - (Int32)item.PREE_QN_ESTOQUE;
                    tipo = 1;
                }
                else
                {
                    quant = (Int32)item.PREE_QN_ESTOQUE - (Int32)item.PREF_QN_QUANTIDADE_ALTERADA;
                    tipo = 2;
                }

                MOVIMENTO_ESTOQUE_PRODUTO movto = new MOVIMENTO_ESTOQUE_PRODUTO();
                movto.ASSI_CD_ID = usuario.ASSI_CD_ID;
                movto.EMPR_CD_ID = item.EMPR_CD_ID;
                movto.MOEP_DT_MOVIMENTO = DateTime.Today;
                movto.MOEP_IN_ATIVO = 1;
                movto.MOEP_IN_CHAVE_ORIGEM = 0;
                movto.MOEP_IN_OPERACAO = operacao;
                movto.MOEP_IN_ORIGEM = "Acerto Manual";
                movto.MOEP_IN_TIPO_MOVIMENTO = tipo;
                movto.MOEP_QN_QUANTIDADE = quant;
                movto.PROD_CD_ID = item.PROD_CD_ID;
                movto.USUA_CD_ID = usuario.USUA_CD_ID;
                movto.ASSI_CD_ID = usuario.ASSI_CD_ID;
                movto.MOEP_QN_ANTES = Convert.ToInt32(item.PREE_QN_ESTOQUE);
                movto.MOEP_QN_ALTERADA = Convert.ToInt32(item.PREE_QN_ESTOQUE - item.PREF_QN_QUANTIDADE_ALTERADA);
                movto.MOEP_QN_DEPOIS = quant;
                movto.MOEP_DS_JUSTIFICATIVA = item.PREE_DS_JUSTIFICATIVA;

                // Persiste estoque
                Int32 volta = _movService.Create(movto);

                item.PREE_QN_ESTOQUE = item.PREF_QN_QUANTIDADE_ALTERADA.Value;
                item.PREE_DT_ULTIMO_MOVIMENTO = DateTime.Now.Date;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
