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
    
    public partial class EMPRESA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EMPRESA()
        {
            this.CLIENTE = new HashSet<CLIENTE>();
            this.CUSTO_FIXO = new HashSet<CUSTO_FIXO>();
            this.EMPRESA_ANEXO = new HashSet<EMPRESA_ANEXO>();
            this.FORNECEDOR = new HashSet<FORNECEDOR>();
            this.FORMA_PAGTO_RECTO = new HashSet<FORMA_PAGTO_RECTO>();
            this.PRODUTO_ESTOQUE_EMPRESA = new HashSet<PRODUTO_ESTOQUE_EMPRESA>();
            this.PRODUTO_TABELA_PRECO = new HashSet<PRODUTO_TABELA_PRECO>();
            this.EMPRESA_MAQUINA = new HashSet<EMPRESA_MAQUINA>();
            this.TRANSPORTADORA = new HashSet<TRANSPORTADORA>();
        }
    
        public int EMPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int RETR_CD_ID { get; set; }
        public Nullable<int> MAQN_CD_ID { get; set; }
        public string EMPR_NM_NOME { get; set; }
        public Nullable<decimal> EMPR_VL_PATRIMONIO_LIQUIDO { get; set; }
        public int EMPR_IN_OPERA_CARTAO { get; set; }
        public string EMPR_NM_OUTRA_MAQUINA { get; set; }
        public Nullable<decimal> EMPR_PC_ANTECIPACAO { get; set; }
        public int EMPR_IN_PAGA_COMISSAO { get; set; }
        public Nullable<decimal> EMPR_VL_IMPOSTO_MEI { get; set; }
        public System.DateTime EMPR_DT_CADASTRO { get; set; }
        public int EMPR_IN_ATIVO { get; set; }
        public Nullable<decimal> EMPR_PC_VENDA_DEBITO { get; set; }
        public Nullable<decimal> EMPR_PC_VENDA_CREDITO { get; set; }
        public Nullable<decimal> EMPR_PC_VENDA_DINHEIRO { get; set; }
        public string EMPR_NM_RAZAO { get; set; }
        public string EMPR_NR_CNPJ { get; set; }
        public string EMPR_NR_INSCRICAO_MUNICIPAL { get; set; }
        public string EMPR_NR_INSCRICAO_ESTADUAL { get; set; }
        public string EMPR_NM_ENDERECO { get; set; }
        public string EMPR_NM_NUMERO { get; set; }
        public string EMPR_NM_COMPLEMENTO { get; set; }
        public string EMPR_NM_BAIRRO { get; set; }
        public string EMPR_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public string EMPR_NR_CEP { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CUSTO_FIXO> CUSTO_FIXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_ANEXO> EMPRESA_ANEXO { get; set; }
        public virtual MAQUINA MAQUINA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR> FORNECEDOR { get; set; }
        public virtual REGIME_TRIBUTARIO REGIME_TRIBUTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMA_PAGTO_RECTO> FORMA_PAGTO_RECTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ESTOQUE_EMPRESA> PRODUTO_ESTOQUE_EMPRESA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_TABELA_PRECO> PRODUTO_TABELA_PRECO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_MAQUINA> EMPRESA_MAQUINA { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSPORTADORA> TRANSPORTADORA { get; set; }
    }
}
