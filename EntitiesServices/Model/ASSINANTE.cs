//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ASSINANTE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ASSINANTE()
        {
            this.AGENDA = new HashSet<AGENDA>();
            this.ASSINANTE_ANEXO = new HashSet<ASSINANTE_ANEXO>();
            this.ASSINANTE_ANOTACAO = new HashSet<ASSINANTE_ANOTACAO>();
            this.ASSINANTE_CONSUMO = new HashSet<ASSINANTE_CONSUMO>();
            this.ASSINANTE_PAGAMENTO = new HashSet<ASSINANTE_PAGAMENTO>();
            this.ASSINANTE_PLANO = new HashSet<ASSINANTE_PLANO>();
            this.ASSINANTE_QUADRO_SOCIETARIO = new HashSet<ASSINANTE_QUADRO_SOCIETARIO>();
            this.CATEGORIA_AGENDA = new HashSet<CATEGORIA_AGENDA>();
            this.CATEGORIA_CLIENTE = new HashSet<CATEGORIA_CLIENTE>();
            this.CATEGORIA_FORNECEDOR = new HashSet<CATEGORIA_FORNECEDOR>();
            this.CATEGORIA_NOTIFICACAO = new HashSet<CATEGORIA_NOTIFICACAO>();
            this.CATEGORIA_SERVICO = new HashSet<CATEGORIA_SERVICO>();
            this.CATEGORIA_USUARIO = new HashSet<CATEGORIA_USUARIO>();
            this.CLIENTE = new HashSet<CLIENTE>();
            this.CONFIGURACAO = new HashSet<CONFIGURACAO>();
            this.CONTA_BANCO = new HashSet<CONTA_BANCO>();
            this.CONTA_RECEBER = new HashSet<CONTA_RECEBER>();
            this.CRM_ACAO = new HashSet<CRM_ACAO>();
            this.CRM = new HashSet<CRM>();
            this.CRM_ORIGEM = new HashSet<CRM_ORIGEM>();
            this.CRM_PEDIDO_VENDA = new HashSet<CRM_PEDIDO_VENDA>();
            this.DIARIO_PROCESSO = new HashSet<DIARIO_PROCESSO>();
            this.EMAIL_AGENDAMENTO = new HashSet<EMAIL_AGENDAMENTO>();
            this.FILIAL = new HashSet<FILIAL>();
            this.FORMA_ENVIO = new HashSet<FORMA_ENVIO>();
            this.FORMA_FRETE = new HashSet<FORMA_FRETE>();
            this.FORMA_PAGTO_RECTO = new HashSet<FORMA_PAGTO_RECTO>();
            this.FORNECEDOR = new HashSet<FORNECEDOR>();
            this.FUNIL = new HashSet<FUNIL>();
            this.GRUPO = new HashSet<GRUPO>();
            this.GRUPO_PLANO_CONTA = new HashSet<GRUPO_PLANO_CONTA>();
            this.LOG = new HashSet<LOG>();
            this.MENSAGEM_AUTOMACAO = new HashSet<MENSAGEM_AUTOMACAO>();
            this.MENSAGENS = new HashSet<MENSAGENS>();
            this.MOTIVO_CANCELAMENTO = new HashSet<MOTIVO_CANCELAMENTO>();
            this.MOTIVO_ENCERRAMENTO = new HashSet<MOTIVO_ENCERRAMENTO>();
            this.MOVIMENTO_ESTOQUE_PRODUTO = new HashSet<MOVIMENTO_ESTOQUE_PRODUTO>();
            this.PEDIDO_COMPRA = new HashSet<PEDIDO_COMPRA>();
            this.PLANO_CONTA = new HashSet<PLANO_CONTA>();
            this.SERVICO = new HashSet<SERVICO>();
            this.SUBCATEGORIA_PRODUTO = new HashSet<SUBCATEGORIA_PRODUTO>();
            this.SUBGRUPO_PLANO_CONTA = new HashSet<SUBGRUPO_PLANO_CONTA>();
            this.TAMANHO = new HashSet<TAMANHO>();
            this.TEMPLATE = new HashSet<TEMPLATE>();
            this.TEMPLATE_EMAIL = new HashSet<TEMPLATE_EMAIL>();
            this.TEMPLATE_PROPOSTA = new HashSet<TEMPLATE_PROPOSTA>();
            this.TEMPLATE_SMS = new HashSet<TEMPLATE_SMS>();
            this.TIPO_ACAO = new HashSet<TIPO_ACAO>();
            this.TIPO_CONTRIBUINTE = new HashSet<TIPO_CONTRIBUINTE>();
            this.TIPO_CRM = new HashSet<TIPO_CRM>();
            this.TIPO_TAREFA = new HashSet<TIPO_TAREFA>();
            this.TRANSPORTADORA = new HashSet<TRANSPORTADORA>();
            this.VIDEO = new HashSet<VIDEO>();
            this.BANCO = new HashSet<BANCO>();
            this.CARGO_USUARIO = new HashSet<CARGO_USUARIO>();
            this.CATEGORIA_PRODUTO = new HashSet<CATEGORIA_PRODUTO>();
            this.CATEGORIA_CUSTO_FIXO = new HashSet<CATEGORIA_CUSTO_FIXO>();
            this.EMPRESA = new HashSet<EMPRESA>();
            this.MAQUINA = new HashSet<MAQUINA>();
            this.NOTICIA = new HashSet<NOTICIA>();
            this.NOTIFICACAO = new HashSet<NOTIFICACAO>();
            this.OUTRO_CUSTO_VARIAVEL = new HashSet<OUTRO_CUSTO_VARIAVEL>();
            this.PESSOA_EXTERNA = new HashSet<PESSOA_EXTERNA>();
            this.PLATAFORMA_ENTREGA = new HashSet<PLATAFORMA_ENTREGA>();
            this.PRODUTO = new HashSet<PRODUTO>();
            this.TAREFA = new HashSet<TAREFA>();
            this.USUARIO = new HashSet<USUARIO>();
        }
    
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> TIPE_CD_ID { get; set; }
        public Nullable<int> PLAN_CD_ID { get; set; }
        public string ASSI_NM_NOME { get; set; }
        public string ASSI_NM_RAZAO_SOCIAL { get; set; }
        public string ASSI_NR_CNPJ { get; set; }
        public string ASSI_NR_INSCRICAO_ESTADUAL { get; set; }
        public string ASSI_NR_INSCRICAO_MUNICIPAL { get; set; }
        public System.DateTime ASSI_DT_CADASTRO { get; set; }
        public int ASSI_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> ASSI_DT_INICIO { get; set; }
        public Nullable<int> ASSI_IN_TIPO { get; set; }
        public Nullable<int> ASSI_IN_STATUS { get; set; }
        public string ASSI_NM_EMAIL { get; set; }
        public string ASSI_NR_CPF { get; set; }
        public string ASSI_TX_OBSERVACOES { get; set; }
        public string ASSI_NM_ENDERECO { get; set; }
        public string ASSI_NR_NUMERO { get; set; }
        public string ASSI_NM_COMPLEMENTO { get; set; }
        public string ASSI_NM_BAIRRO { get; set; }
        public string ASSI_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public string ASSI_NR_CEP { get; set; }
        public string ASSI_AQ_FOTO { get; set; }
        public string ASSI_NR_TELEFONE { get; set; }
        public string ASSI_NR_CELULAR { get; set; }
        public Nullable<int> ASSI_IN_BLOQUEADO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_ANEXO> ASSINANTE_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_ANOTACAO> ASSINANTE_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_CONSUMO> ASSINANTE_CONSUMO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_PAGAMENTO> ASSINANTE_PAGAMENTO { get; set; }
        public virtual PLANO PLANO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_PLANO> ASSINANTE_PLANO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_QUADRO_SOCIETARIO> ASSINANTE_QUADRO_SOCIETARIO { get; set; }
        public virtual TIPO_PESSOA TIPO_PESSOA { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_AGENDA> CATEGORIA_AGENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_CLIENTE> CATEGORIA_CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_FORNECEDOR> CATEGORIA_FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_NOTIFICACAO> CATEGORIA_NOTIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_SERVICO> CATEGORIA_SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_USUARIO> CATEGORIA_USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONFIGURACAO> CONFIGURACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_BANCO> CONTA_BANCO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ACAO> CRM_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM> CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ORIGEM> CRM_ORIGEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DIARIO_PROCESSO> DIARIO_PROCESSO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMAIL_AGENDAMENTO> EMAIL_AGENDAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FILIAL> FILIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMA_ENVIO> FORMA_ENVIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMA_FRETE> FORMA_FRETE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMA_PAGTO_RECTO> FORMA_PAGTO_RECTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR> FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FUNIL> FUNIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRUPO> GRUPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRUPO_PLANO_CONTA> GRUPO_PLANO_CONTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOG> LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGEM_AUTOMACAO> MENSAGEM_AUTOMACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOTIVO_CANCELAMENTO> MOTIVO_CANCELAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOTIVO_ENCERRAMENTO> MOTIVO_ENCERRAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_COMPRA> PEDIDO_COMPRA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PLANO_CONTA> PLANO_CONTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SERVICO> SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBCATEGORIA_PRODUTO> SUBCATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBGRUPO_PLANO_CONTA> SUBGRUPO_PLANO_CONTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAMANHO> TAMANHO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE> TEMPLATE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_EMAIL> TEMPLATE_EMAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_PROPOSTA> TEMPLATE_PROPOSTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEMPLATE_SMS> TEMPLATE_SMS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_ACAO> TIPO_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_CONTRIBUINTE> TIPO_CONTRIBUINTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_CRM> TIPO_CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPO_TAREFA> TIPO_TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSPORTADORA> TRANSPORTADORA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VIDEO> VIDEO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BANCO> BANCO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CARGO_USUARIO> CARGO_USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_PRODUTO> CATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CATEGORIA_CUSTO_FIXO> CATEGORIA_CUSTO_FIXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA> EMPRESA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAQUINA> MAQUINA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTICIA> NOTICIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICACAO> NOTIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OUTRO_CUSTO_VARIAVEL> OUTRO_CUSTO_VARIAVEL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESSOA_EXTERNA> PESSOA_EXTERNA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PLATAFORMA_ENTREGA> PLATAFORMA_ENTREGA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA> TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
    }
}
