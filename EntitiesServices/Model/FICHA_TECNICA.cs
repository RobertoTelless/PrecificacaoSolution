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
    
    public partial class FICHA_TECNICA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FICHA_TECNICA()
        {
            this.FICHA_TECNICA1 = new HashSet<FICHA_TECNICA>();
            this.FICHA_TECNICA_DETALHE = new HashSet<FICHA_TECNICA_DETALHE>();
        }
    
        public int FITE_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public int FITE_CD_FICHA_PAI { get; set; }
        public string FITE_AQ_FOTO_APRESENTACAO { get; set; }
        public string FITE_NM_NOME { get; set; }
        public string FITE_DS_DESCRICAO { get; set; }
        public string FITE_DS_APRESENTACAO { get; set; }
        public Nullable<int> FITE_NR_PORCOES { get; set; }
        public Nullable<decimal> FITE_NR_PESO { get; set; }
        public Nullable<decimal> FITE_NR_FATOR_CORRECAO { get; set; }
        public Nullable<decimal> FITE_QN_QUANTIDADE_LIQUIDA { get; set; }
        public Nullable<decimal> FITE_PC_PERCENTUAL_PERDA { get; set; }
        public Nullable<System.DateTime> FITE_DT_CADASTRO { get; set; }
        public Nullable<int> FITE_IN_ATIVO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FICHA_TECNICA> FICHA_TECNICA1 { get; set; }
        public virtual FICHA_TECNICA FICHA_TECNICA2 { get; set; }
        public virtual PRODUTO_TABELA_PRECO PRODUTO_TABELA_PRECO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FICHA_TECNICA_DETALHE> FICHA_TECNICA_DETALHE { get; set; }
    }
}
