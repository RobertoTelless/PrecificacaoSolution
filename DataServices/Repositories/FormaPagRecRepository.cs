using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class FormaPagRecRepository : RepositoryBase<FORMA_PAGTO_RECTO>, IFormaPagRecRepository
    {
        public FORMA_PAGTO_RECTO CheckExist(FORMA_PAGTO_RECTO tarefa, Int32 idUsu)
        {
            IQueryable<FORMA_PAGTO_RECTO> query = Db.FORMA_PAGTO_RECTO;
            query = query.Where(p => p.FOPR_NM_NOME_FORMA == tarefa.FOPR_NM_NOME_FORMA);
            query = query.Where(p => p.ASSI_CD_ID == tarefa.ASSI_CD_ID);
            return query.FirstOrDefault();
        }

        public FORMA_PAGTO_RECTO GetItemById(Int32 id)
        {
            IQueryable<FORMA_PAGTO_RECTO> query = Db.FORMA_PAGTO_RECTO;
            query = query.Where(p => p.FOPR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<FORMA_PAGTO_RECTO> GetAllItens(Int32 idUsu)
        {
            IQueryable<FORMA_PAGTO_RECTO> query = Db.FORMA_PAGTO_RECTO.Where(p => p.FOPR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<FORMA_PAGTO_RECTO> GetAllItensAdm(Int32 idUsu)
        {
            IQueryable<FORMA_PAGTO_RECTO> query = Db.FORMA_PAGTO_RECTO;
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<FORMA_PAGTO_RECTO> ExecuteFilter(Int32? tipo, Int32? conta, String nome, Int32? idAss)
        {
            List<FORMA_PAGTO_RECTO> lista = new List<FORMA_PAGTO_RECTO>();
            IQueryable<FORMA_PAGTO_RECTO> query = Db.FORMA_PAGTO_RECTO;
            if (tipo > 0)
            {
                query = query.Where(p => p.FOPA_IN_TIPO_FORMA == tipo);
            }
            if (conta > 0)
            {
                query = query.Where(p => p.COBA_CD_ID == conta);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.FOPR_NM_NOME_FORMA.Contains(nome));
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.FOPR_NM_NOME_FORMA);
                lista = query.ToList<FORMA_PAGTO_RECTO>();
            }
            return lista;
        }
    }
}
 