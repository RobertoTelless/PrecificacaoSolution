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
    
    public partial class USUARIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USUARIO()
        {
            this.AGENDA = new HashSet<AGENDA>();
            this.AGENDA1 = new HashSet<AGENDA>();
            this.FORNECEDOR_ANOTACOES = new HashSet<FORNECEDOR_ANOTACOES>();
            this.LOG = new HashSet<LOG>();
            this.NOTICIA_COMENTARIO = new HashSet<NOTICIA_COMENTARIO>();
            this.NOTIFICACAO = new HashSet<NOTIFICACAO>();
            this.PESSOA_EXTERNA_ANOTACAO = new HashSet<PESSOA_EXTERNA_ANOTACAO>();
            this.TAREFA = new HashSet<TAREFA>();
            this.TAREFA_ACOMPANHAMENTO = new HashSet<TAREFA_ACOMPANHAMENTO>();
            this.USUARIO_ANEXO = new HashSet<USUARIO_ANEXO>();
            this.VIDEO_COMENTARIO = new HashSet<VIDEO_COMENTARIO>();
            this.USUARIO_ANOTACAO = new HashSet<USUARIO_ANOTACAO>();
            this.USUARIO_ANOTACAO1 = new HashSet<USUARIO_ANOTACAO>();
            this.CLIENTE = new HashSet<CLIENTE>();
            this.CLIENTE_ANOTACAO = new HashSet<CLIENTE_ANOTACAO>();
        }
    
        public int USUA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int PERF_CD_ID { get; set; }
        public int CAUS_CD_ID { get; set; }
        public Nullable<int> CARG_CD_ID { get; set; }
        public string USUA_AQ_FOTO { get; set; }
        public string USUA_NM_NOME { get; set; }
        public string USUA_NM_LOGIN { get; set; }
        public string USUA_EM_EMAIL { get; set; }
        public string USUA_NR_CPF { get; set; }
        public string USUA_NR_RG { get; set; }
        public string USUA_NR_TELEFONE { get; set; }
        public string USUA_NR_CELULAR { get; set; }
        public string USUA_NM_SENHA { get; set; }
        public string USUA_NM_SENHA_CONFIRMA { get; set; }
        public string USUA_NM_NOVA_SENHA { get; set; }
        public int USUA_IN_BLOQUEADO { get; set; }
        public int USUA_IN_PROVISORIO { get; set; }
        public int USUA_IN_LOGIN_PROVISORIO { get; set; }
        public int USUA_IN_ATIVO { get; set; }
        public int USUA_IN_LOGADO { get; set; }
        public Nullable<System.DateTime> USUA_DT_BLOQUEADO { get; set; }
        public Nullable<System.DateTime> USUA_DT_ALTERACAO { get; set; }
        public Nullable<System.DateTime> USUA_DT_TROCA_SENHA { get; set; }
        public Nullable<System.DateTime> USUA_DT_ACESSO { get; set; }
        public Nullable<System.DateTime> USUA_DT_ULTIMA_FALHA { get; set; }
        public System.DateTime USUA_DT_CADASTRO { get; set; }
        public int USUA_NR_ACESSOS { get; set; }
        public int USUA_NR_FALHAS { get; set; }
        public string USUA_TX_OBSERVACOES { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGENDA> AGENDA1 { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CARGO_USUARIO CARGO_USUARIO { get; set; }
        public virtual CATEGORIA_USUARIO CATEGORIA_USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR_ANOTACOES> FORNECEDOR_ANOTACOES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOG> LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTICIA_COMENTARIO> NOTICIA_COMENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICACAO> NOTIFICACAO { get; set; }
        public virtual PERFIL PERFIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESSOA_EXTERNA_ANOTACAO> PESSOA_EXTERNA_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA> TAREFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_ACOMPANHAMENTO> TAREFA_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO_ANEXO> USUARIO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VIDEO_COMENTARIO> VIDEO_COMENTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO_ANOTACAO> USUARIO_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO_ANOTACAO> USUARIO_ANOTACAO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_ANOTACAO> CLIENTE_ANOTACAO { get; set; }
    }
}
