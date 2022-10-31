﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Db_PrecificacaoEntities : DbContext
    {
        public Db_PrecificacaoEntities()
            : base("name=Db_PrecificacaoEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AGENDA> AGENDA { get; set; }
        public virtual DbSet<AGENDA_ANEXO> AGENDA_ANEXO { get; set; }
        public virtual DbSet<ASSINANTE> ASSINANTE { get; set; }
        public virtual DbSet<BANCO> BANCO { get; set; }
        public virtual DbSet<CARGO_USUARIO> CARGO_USUARIO { get; set; }
        public virtual DbSet<CATEGORIA_AGENDA> CATEGORIA_AGENDA { get; set; }
        public virtual DbSet<CATEGORIA_CLIENTE> CATEGORIA_CLIENTE { get; set; }
        public virtual DbSet<CATEGORIA_CUSTO_FIXO> CATEGORIA_CUSTO_FIXO { get; set; }
        public virtual DbSet<CATEGORIA_FORNECEDOR> CATEGORIA_FORNECEDOR { get; set; }
        public virtual DbSet<CATEGORIA_NOTIFICACAO> CATEGORIA_NOTIFICACAO { get; set; }
        public virtual DbSet<CATEGORIA_PRODUTO> CATEGORIA_PRODUTO { get; set; }
        public virtual DbSet<CATEGORIA_USUARIO> CATEGORIA_USUARIO { get; set; }
        public virtual DbSet<CLIENTE> CLIENTE { get; set; }
        public virtual DbSet<CLIENTE_ANEXO> CLIENTE_ANEXO { get; set; }
        public virtual DbSet<CLIENTE_ANOTACAO> CLIENTE_ANOTACAO { get; set; }
        public virtual DbSet<CLIENTE_CONTATO> CLIENTE_CONTATO { get; set; }
        public virtual DbSet<CLIENTE_QUADRO_SOCIETARIO> CLIENTE_QUADRO_SOCIETARIO { get; set; }
        public virtual DbSet<CLIENTE_REFERENCIA> CLIENTE_REFERENCIA { get; set; }
        public virtual DbSet<COMISSAO_CARGO> COMISSAO_CARGO { get; set; }
        public virtual DbSet<CONFIGURACAO> CONFIGURACAO { get; set; }
        public virtual DbSet<CONTA_BANCO> CONTA_BANCO { get; set; }
        public virtual DbSet<CONTA_BANCO_ANEXO> CONTA_BANCO_ANEXO { get; set; }
        public virtual DbSet<CONTA_BANCO_CONTATO> CONTA_BANCO_CONTATO { get; set; }
        public virtual DbSet<CONTA_BANCO_LANCAMENTO> CONTA_BANCO_LANCAMENTO { get; set; }
        public virtual DbSet<CRM> CRM { get; set; }
        public virtual DbSet<CRM_ACAO> CRM_ACAO { get; set; }
        public virtual DbSet<CRM_ANEXO> CRM_ANEXO { get; set; }
        public virtual DbSet<CRM_COMENTARIO> CRM_COMENTARIO { get; set; }
        public virtual DbSet<CRM_CONTATO> CRM_CONTATO { get; set; }
        public virtual DbSet<CRM_ORIGEM> CRM_ORIGEM { get; set; }
        public virtual DbSet<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        public virtual DbSet<CRM_PEDIDO_VENDA_ACOMPANHAMENTO> CRM_PEDIDO_VENDA_ACOMPANHAMENTO { get; set; }
        public virtual DbSet<CRM_PEDIDO_VENDA_ANEXO> CRM_PEDIDO_VENDA_ANEXO { get; set; }
        public virtual DbSet<CRM_PEDIDO_VENDA_ITEM> CRM_PEDIDO_VENDA_ITEM { get; set; }
        public virtual DbSet<CUSTO_FIXO> CUSTO_FIXO { get; set; }
        public virtual DbSet<EMPRESA> EMPRESA { get; set; }
        public virtual DbSet<EMPRESA_ANEXO> EMPRESA_ANEXO { get; set; }
        public virtual DbSet<EMPRESA_MAQUINA> EMPRESA_MAQUINA { get; set; }
        public virtual DbSet<FICHA_TECNICA> FICHA_TECNICA { get; set; }
        public virtual DbSet<FICHA_TECNICA_DETALHE> FICHA_TECNICA_DETALHE { get; set; }
        public virtual DbSet<FORMA_ENVIO> FORMA_ENVIO { get; set; }
        public virtual DbSet<FORMA_FRETE> FORMA_FRETE { get; set; }
        public virtual DbSet<FORMA_PAGTO_RECTO> FORMA_PAGTO_RECTO { get; set; }
        public virtual DbSet<FORNECEDOR> FORNECEDOR { get; set; }
        public virtual DbSet<FORNECEDOR_ANEXO> FORNECEDOR_ANEXO { get; set; }
        public virtual DbSet<FORNECEDOR_ANOTACOES> FORNECEDOR_ANOTACOES { get; set; }
        public virtual DbSet<FORNECEDOR_CONTATO> FORNECEDOR_CONTATO { get; set; }
        public virtual DbSet<FORNECEDOR_QUADRO_SOCIETARIO> FORNECEDOR_QUADRO_SOCIETARIO { get; set; }
        public virtual DbSet<GRUPO_PLANO_CONTA> GRUPO_PLANO_CONTA { get; set; }
        public virtual DbSet<LOG> LOG { get; set; }
        public virtual DbSet<MAQUINA> MAQUINA { get; set; }
        public virtual DbSet<MOTIVO_CANCELAMENTO> MOTIVO_CANCELAMENTO { get; set; }
        public virtual DbSet<MOTIVO_ENCERRAMENTO> MOTIVO_ENCERRAMENTO { get; set; }
        public virtual DbSet<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        public virtual DbSet<NOTICIA> NOTICIA { get; set; }
        public virtual DbSet<NOTICIA_COMENTARIO> NOTICIA_COMENTARIO { get; set; }
        public virtual DbSet<NOTIFICACAO> NOTIFICACAO { get; set; }
        public virtual DbSet<NOTIFICACAO_ANEXO> NOTIFICACAO_ANEXO { get; set; }
        public virtual DbSet<OUTRO_CUSTO_VARIAVEL> OUTRO_CUSTO_VARIAVEL { get; set; }
        public virtual DbSet<PERFIL> PERFIL { get; set; }
        public virtual DbSet<PERIODICIDADE_TAREFA> PERIODICIDADE_TAREFA { get; set; }
        public virtual DbSet<PESSOA_EXTERNA> PESSOA_EXTERNA { get; set; }
        public virtual DbSet<PESSOA_EXTERNA_ANEXO> PESSOA_EXTERNA_ANEXO { get; set; }
        public virtual DbSet<PESSOA_EXTERNA_ANOTACAO> PESSOA_EXTERNA_ANOTACAO { get; set; }
        public virtual DbSet<PLANO_CONTA> PLANO_CONTA { get; set; }
        public virtual DbSet<PLATAFORMA_ENTREGA> PLATAFORMA_ENTREGA { get; set; }
        public virtual DbSet<PRODUTO> PRODUTO { get; set; }
        public virtual DbSet<PRODUTO_ANEXO> PRODUTO_ANEXO { get; set; }
        public virtual DbSet<PRODUTO_ESTOQUE_EMPRESA> PRODUTO_ESTOQUE_EMPRESA { get; set; }
        public virtual DbSet<PRODUTO_FORNECEDOR> PRODUTO_FORNECEDOR { get; set; }
        public virtual DbSet<PRODUTO_KIT> PRODUTO_KIT { get; set; }
        public virtual DbSet<PRODUTO_ORIGEM> PRODUTO_ORIGEM { get; set; }
        public virtual DbSet<PRODUTO_TABELA_PRECO> PRODUTO_TABELA_PRECO { get; set; }
        public virtual DbSet<PRODUTO_ULTIMOS_CUSTOS> PRODUTO_ULTIMOS_CUSTOS { get; set; }
        public virtual DbSet<REGIME_TRIBUTARIO> REGIME_TRIBUTARIO { get; set; }
        public virtual DbSet<SEXO> SEXO { get; set; }
        public virtual DbSet<SUBCATEGORIA_PRODUTO> SUBCATEGORIA_PRODUTO { get; set; }
        public virtual DbSet<SUBGRUPO_PLANO_CONTA> SUBGRUPO_PLANO_CONTA { get; set; }
        public virtual DbSet<TAMANHO> TAMANHO { get; set; }
        public virtual DbSet<TAREFA> TAREFA { get; set; }
        public virtual DbSet<TAREFA_ACOMPANHAMENTO> TAREFA_ACOMPANHAMENTO { get; set; }
        public virtual DbSet<TAREFA_ANEXO> TAREFA_ANEXO { get; set; }
        public virtual DbSet<TEMPLATE> TEMPLATE { get; set; }
        public virtual DbSet<TEMPLATE_EMAIL> TEMPLATE_EMAIL { get; set; }
        public virtual DbSet<TEMPLATE_PROPOSTA> TEMPLATE_PROPOSTA { get; set; }
        public virtual DbSet<TEMPLATE_SMS> TEMPLATE_SMS { get; set; }
        public virtual DbSet<TIPO_ACAO> TIPO_ACAO { get; set; }
        public virtual DbSet<TIPO_CONTA> TIPO_CONTA { get; set; }
        public virtual DbSet<TIPO_CONTRIBUINTE> TIPO_CONTRIBUINTE { get; set; }
        public virtual DbSet<TIPO_CRM> TIPO_CRM { get; set; }
        public virtual DbSet<TIPO_PESSOA> TIPO_PESSOA { get; set; }
        public virtual DbSet<TIPO_TAREFA> TIPO_TAREFA { get; set; }
        public virtual DbSet<TIPO_TRANSPORTE> TIPO_TRANSPORTE { get; set; }
        public virtual DbSet<TIPO_VEICULO> TIPO_VEICULO { get; set; }
        public virtual DbSet<TRANSPORTADORA> TRANSPORTADORA { get; set; }
        public virtual DbSet<TRANSPORTADORA_ANEXO> TRANSPORTADORA_ANEXO { get; set; }
        public virtual DbSet<UF> UF { get; set; }
        public virtual DbSet<UNIDADE> UNIDADE { get; set; }
        public virtual DbSet<USUARIO> USUARIO { get; set; }
        public virtual DbSet<USUARIO_ANEXO> USUARIO_ANEXO { get; set; }
        public virtual DbSet<USUARIO_ANOTACAO> USUARIO_ANOTACAO { get; set; }
        public virtual DbSet<VIDEO> VIDEO { get; set; }
        public virtual DbSet<VIDEO_COMENTARIO> VIDEO_COMENTARIO { get; set; }
        public virtual DbSet<TIPO_EMBALAGEM> TIPO_EMBALAGEM { get; set; }
        public virtual DbSet<FUNIL> FUNIL { get; set; }
        public virtual DbSet<FUNIL_ETAPA> FUNIL_ETAPA { get; set; }
        public virtual DbSet<CONTA_PAGAR> CONTA_PAGAR { get; set; }
        public virtual DbSet<CONTA_PAGAR_ANEXO> CONTA_PAGAR_ANEXO { get; set; }
        public virtual DbSet<CONTA_PAGAR_PARCELA> CONTA_PAGAR_PARCELA { get; set; }
        public virtual DbSet<CONTA_PAGAR_RATEIO> CONTA_PAGAR_RATEIO { get; set; }
        public virtual DbSet<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        public virtual DbSet<CONTA_RECEBER_ANEXO> CONTA_RECEBER_ANEXO { get; set; }
        public virtual DbSet<CONTA_RECEBER_PARCELA> CONTA_RECEBER_PARCELA { get; set; }
        public virtual DbSet<CONTA_RECEBER_RATEIO> CONTA_RECEBER_RATEIO { get; set; }
        public virtual DbSet<CATEGORIA_SERVICO> CATEGORIA_SERVICO { get; set; }
        public virtual DbSet<FILIAL> FILIAL { get; set; }
        public virtual DbSet<NOMENCLATURA_BRAS_SERVICOS> NOMENCLATURA_BRAS_SERVICOS { get; set; }
        public virtual DbSet<SERVICO> SERVICO { get; set; }
        public virtual DbSet<SERVICO_ANEXO> SERVICO_ANEXO { get; set; }
        public virtual DbSet<SERVICO_TABELA_PRECO> SERVICO_TABELA_PRECO { get; set; }
    }
}
