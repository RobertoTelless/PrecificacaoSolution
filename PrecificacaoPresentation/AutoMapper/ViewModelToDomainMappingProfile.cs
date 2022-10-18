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

        }
    }
}