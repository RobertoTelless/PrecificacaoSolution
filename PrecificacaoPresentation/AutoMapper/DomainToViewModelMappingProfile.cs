using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<USUARIO, UsuarioViewModel>();
            CreateMap<USUARIO, UsuarioLoginViewModel>();
            CreateMap<LOG, LogViewModel>();
            CreateMap<CONFIGURACAO, ConfiguracaoViewModel>();
            CreateMap<CARGO_USUARIO, CargoViewModel>();
            CreateMap<NOTICIA, NoticiaViewModel>();
            CreateMap<NOTICIA_COMENTARIO, NoticiaComentarioViewModel>();
            CreateMap<NOTIFICACAO, NotificacaoViewModel>();
            CreateMap<TEMPLATE, TemplateViewModel>();
            CreateMap<TAREFA, TarefaViewModel>();
            CreateMap<CATEGORIA_AGENDA, CategoriaAgendaViewModel>();
            CreateMap<AGENDA, AgendaViewModel>();
            CreateMap<TAREFA_ACOMPANHAMENTO, TarefaAcompanhamentoViewModel>();
            CreateMap<CATEGORIA_NOTIFICACAO, CategoriaNotificacaoViewModel>();
            CreateMap<CATEGORIA_USUARIO, CategoriaUsuarioViewModel>();
            CreateMap<EMPRESA, EmpresaViewModel>();
            CreateMap<FORMA_PAGTO_RECTO, FormaPagRecViewModel>();
            CreateMap<MAQUINA, MaquinaViewModel>();
            CreateMap<PESSOA_EXTERNA, PessoaExternaViewModel>();
            CreateMap<PESSOA_EXTERNA_ANOTACAO, PessoaExternaAnotacaoViewModel>();
            CreateMap<PLATAFORMA_ENTREGA, PlataformaEntregaViewModel>();
            CreateMap<USUARIO_ANOTACAO, UsuarioAnotacaoViewModel>();
            CreateMap<PLANO_CONTA, CentroCustoViewModel>();
            CreateMap<GRUPO_PLANO_CONTA, GrupoViewModel>();
            CreateMap<SUBGRUPO_PLANO_CONTA, SubgrupoViewModel>();
            CreateMap<BANCO, BancoViewModel>();
            CreateMap<CONTA_BANCO, ContaBancariaViewModel>();
            CreateMap<CONTA_BANCO_CONTATO, ContaBancariaContatoViewModel>();
            CreateMap<CONTA_BANCO_LANCAMENTO, ContaBancariaLancamentoViewModel>();
            CreateMap<VIDEO, VideoViewModel>();
            CreateMap<VIDEO_COMENTARIO, VideoComentarioViewModel>();
            CreateMap<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>();
            CreateMap<FORNECEDOR, FornecedorViewModel>();
            CreateMap<FORNECEDOR_CONTATO, FornecedorContatoViewModel>();
            CreateMap<FORNECEDOR_ANOTACOES, FornecedorAnotacaoViewModel>();
            CreateMap<CATEGORIA_CLIENTE, CategoriaClienteViewModel>();
            CreateMap<CLIENTE, ClienteViewModel>();
            CreateMap<CLIENTE_CONTATO, ClienteContatoViewModel>();
            CreateMap<CLIENTE_REFERENCIA, ClienteReferenciaViewModel>();
            CreateMap<EMPRESA_MAQUINA, EmpresaMaquinaViewModel>();
            CreateMap<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>();
            CreateMap<TAMANHO, TamanhoViewModel>();
            CreateMap<UNIDADE, UnidadeViewModel>();
            CreateMap<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>();
            //CreateMap<PRODUTO_BARCODE, ProdutoBarcodeViewModel>();
            CreateMap<PRODUTO_FORNECEDOR, ProdutoFornecedorViewModel>();
            //CreateMap<PRODUTO_GRADE, ProdutoGradeViewModel>();
            CreateMap<PRODUTO_TABELA_PRECO, ProdutoTabelaPrecoViewModel>();
            CreateMap<PRODUTO, ProdutoViewModel>();
            //CreateMap<FICHA_TECNICA, FichaTecnicaViewModel>();
            //CreateMap<FICHA_TECNICA_DETALHE, FichaTecnicaDetalheViewModel>();
            CreateMap<PRODUTO_KIT, ProdutoKitViewModel>();
            CreateMap<TIPO_EMBALAGEM, TipoEmbalagemViewModel>();
            CreateMap<MOVIMENTO_ESTOQUE_PRODUTO, MovimentoEstoqueProdutoViewModel>();
            CreateMap<FICHA_TECNICA, FichaTecnicaViewModel>();
            CreateMap<FICHA_TECNICA_DETALHE, FichaTecnicaDetalheViewModel>();
            CreateMap<TIPO_ACAO, TipoAcaoViewModel>();
            CreateMap<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>();
            CreateMap<MOTIVO_ENCERRAMENTO, MotivoEncerramentoViewModel>();
            CreateMap<CONTA_PAGAR, ContaPagarViewModel>();
            CreateMap<CONTA_PAGAR_PARCELA, ContaPagarParcelaViewModel>();
            CreateMap<CONTA_PAGAR_RATEIO, ContaPagarRateioViewModel>();
            CreateMap<CONTA_RECEBER, ContaReceberViewModel>();
            CreateMap<CONTA_RECEBER_PARCELA, ContaReceberParcelaViewModel>();
            CreateMap<CRM, CRMViewModel>();
            CreateMap<CRM_ACAO, CRMAcaoViewModel>();
            CreateMap<CRM_COMENTARIO, CRMComentarioViewModel>();
            CreateMap<CRM_CONTATO, CRMContatoViewModel>();
            CreateMap<CRM_PEDIDO_VENDA_ITEM, CRMItemPedidoViewModel>();
            CreateMap<CRM_ORIGEM, CRMOrigemViewModel>();
            CreateMap<CRM_PEDIDO_VENDA_ACOMPANHAMENTO, CRMPedidoComentarioViewModel>();
            CreateMap<CRM_PEDIDO_VENDA, CRMPedidoViewModel>();
            CreateMap<CATEGORIA_SERVICO, CategoriaServicoViewModel>();
            CreateMap<SERVICO, ServicoViewModel>();
            CreateMap<TRANSPORTADORA, TransportadoraViewModel>();
            CreateMap<TEMPLATE_EMAIL, TemplateEMailViewModel>();
            CreateMap<TEMPLATE_SMS, TemplateSMSViewModel>();
            CreateMap<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>();
            CreateMap<PEDIDO_COMPRA, PedidoCompraViewModel>();
            CreateMap<PEDIDO_COMPRA_ACOMPANHAMENTO, PedidoCompraAcompanhamentoViewModel>();
            CreateMap<ASSINANTE_ANOTACAO, AssinanteAnotacaoViewModel>();
            CreateMap<ASSINANTE_PAGAMENTO, AssinantePagamentoViewModel>();
            CreateMap<ASSINANTE_PLANO, AssinantePlanoViewModel>();
            CreateMap<PLANO, PlanoViewModel>();
            CreateMap<ASSINANTE, AssinanteViewModel>();
            CreateMap<CUSTO_FIXO, CustoFixoViewModel>();
            CreateMap<CATEGORIA_CUSTO_FIXO, CategoriaCustoFixoViewModel>();
            CreateMap<EMPRESA_CUSTO_VARIAVEL, EmpresaOutorsCustosViewModel>();
            CreateMap<EMPRESA_PLATAFORMA, EmpresaPlataformaViewModel>();


        }
    }
}
