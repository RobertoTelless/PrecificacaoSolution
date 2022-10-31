using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<UsuarioViewModel, USUARIO>();
            CreateMap<UsuarioLoginViewModel, USUARIO>();
            CreateMap<LogViewModel, LOG>();
            CreateMap<ConfiguracaoViewModel, CONFIGURACAO>();
            CreateMap<CargoViewModel, CARGO_USUARIO>();
            CreateMap<NoticiaViewModel, NOTICIA>();
            CreateMap<NoticiaComentarioViewModel, NOTICIA_COMENTARIO>();
            CreateMap<NotificacaoViewModel, NOTIFICACAO>();
            CreateMap<TemplateViewModel, TEMPLATE>();
            CreateMap<TarefaViewModel, TAREFA>();
            CreateMap<CategoriaAgendaViewModel, CATEGORIA_AGENDA>();
            CreateMap<AgendaViewModel, AGENDA>();
            CreateMap<TarefaAcompanhamentoViewModel, TAREFA_ACOMPANHAMENTO>();
            CreateMap<CategoriaNotificacaoViewModel, CATEGORIA_NOTIFICACAO>();
            CreateMap<CategoriaUsuarioViewModel, CATEGORIA_USUARIO>();
            CreateMap<EmpresaViewModel, EMPRESA>();
            CreateMap<FormaPagRecViewModel, FORMA_PAGTO_RECTO>();
            CreateMap<MaquinaViewModel, MAQUINA>();
            CreateMap<PessoaExternaViewModel, PESSOA_EXTERNA>();
            CreateMap<PessoaExternaAnotacaoViewModel, PESSOA_EXTERNA_ANOTACAO>();
            CreateMap<PlataformaEntregaViewModel, PLATAFORMA_ENTREGA>();
            CreateMap<UsuarioAnotacaoViewModel, USUARIO_ANOTACAO>();
            CreateMap<CentroCustoViewModel, PLANO_CONTA>();
            CreateMap<GrupoViewModel, GRUPO_PLANO_CONTA>();
            CreateMap<SubgrupoViewModel, SUBGRUPO_PLANO_CONTA>();
            CreateMap<BancoViewModel, BANCO>();
            CreateMap<ContaBancariaViewModel, CONTA_BANCO>();
            CreateMap<ContaBancariaContatoViewModel, CONTA_BANCO_CONTATO>();
            CreateMap<ContaBancariaLancamentoViewModel, CONTA_BANCO_LANCAMENTO>();
            CreateMap<VideoViewModel, VIDEO>();
            CreateMap<VideoComentarioViewModel, VIDEO_COMENTARIO>();
            CreateMap<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>();
            CreateMap<FornecedorViewModel, FORNECEDOR>();
            CreateMap<FornecedorContatoViewModel, FORNECEDOR_CONTATO>();
            CreateMap<FornecedorAnotacaoViewModel, FORNECEDOR_ANOTACOES>();
            CreateMap<CategoriaClienteViewModel, CATEGORIA_CLIENTE>();
            CreateMap<ClienteReferenciaViewModel, CLIENTE_REFERENCIA>();
            CreateMap<ClienteContatoViewModel, CLIENTE_CONTATO>();
            CreateMap<ClienteViewModel, CLIENTE>();
            CreateMap<EmpresaMaquinaViewModel, EMPRESA_MAQUINA>();
            CreateMap<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>();
            CreateMap<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>();
            CreateMap<UnidadeViewModel, UNIDADE>();
            CreateMap<TamanhoViewModel, TAMANHO>();
            //CreateMap<ProdutoBarcodeViewModel, PRODUTO_BARCODE>();
            CreateMap<ProdutoFornecedorViewModel, PRODUTO_FORNECEDOR>();
            //CreateMap<ProdutoGradeViewModel, PRODUTO_GRADE>();
            CreateMap<ProdutoTabelaPrecoViewModel, PRODUTO_TABELA_PRECO>();
            CreateMap<ProdutoViewModel, PRODUTO>();
            CreateMap<ProdutoKitViewModel, PRODUTO_KIT>();
            CreateMap<TipoEmbalagemViewModel, TIPO_EMBALAGEM>();
            CreateMap<MovimentoEstoqueProdutoViewModel, MOVIMENTO_ESTOQUE_PRODUTO>();
            CreateMap<FichaTecnicaViewModel, FICHA_TECNICA>();
            CreateMap<FichaTecnicaDetalheViewModel, FICHA_TECNICA_DETALHE>();
            CreateMap<TipoAcaoViewModel, TIPO_ACAO>();
            CreateMap<MotivoEncerramentoViewModel, MOTIVO_ENCERRAMENTO>();
            CreateMap<MotivoCancelamentoViewModel, MOTIVO_CANCELAMENTO>();
            CreateMap<ContaPagarViewModel, CONTA_PAGAR>();
            CreateMap<ContaPagarParcelaViewModel, CONTA_PAGAR_PARCELA>();
            CreateMap<ContaPagarRateioViewModel, CONTA_PAGAR_RATEIO>();
            CreateMap<ContaReceberViewModel, CONTA_RECEBER>();
            CreateMap<ContaReceberParcelaViewModel, CONTA_RECEBER_PARCELA>();
            CreateMap<CRMViewModel, CRM>();
            CreateMap<CRMAcaoViewModel, CRM_ACAO>();
            CreateMap<CRMComentarioViewModel, CRM_COMENTARIO>();
            CreateMap<CRMContatoViewModel, CRM_CONTATO>();
            CreateMap<CRMItemPedidoViewModel, CRM_PEDIDO_VENDA_ITEM>();
            CreateMap<CRMOrigemViewModel, CRM_ORIGEM>();
            CreateMap<CRMPedidoComentarioViewModel, CRM_PEDIDO_VENDA_ACOMPANHAMENTO>();
            CreateMap<CRMPedidoViewModel, CRM_PEDIDO_VENDA>();
            CreateMap<CategoriaServicoViewModel, CATEGORIA_SERVICO>();
            CreateMap<ServicoViewModel, SERVICO>();
            CreateMap<TransportadoraViewModel, TRANSPORTADORA>();
            CreateMap<TemplateEMailViewModel, TEMPLATE_EMAIL>();
            CreateMap<TemplateSMSViewModel, TEMPLATE_SMS>();

        }
    }
}