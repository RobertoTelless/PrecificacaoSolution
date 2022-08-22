using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class CentroCustoRepository : RepositoryBase<PLANO_CONTA>, ICentroCustoRepository
    {
        public PLANO_CONTA CheckExist(PLANO_CONTA conta, Int32 idAss)
        {
            IQueryable<PLANO_CONTA> query = Db.PLANO_CONTA;
            query = query.Where(p => p.CECU_IN_TIPO == conta.CECU_IN_TIPO);
            query = query.Where(p => p.CECU_NR_NUMERO == conta.CECU_NR_NUMERO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public PLANO_CONTA GetByNome(String nome, Int32 idAss)
        {
            IQueryable<PLANO_CONTA> query = Db.PLANO_CONTA.Where(p => p.CECU_IN_ATIVO == 1);
            query = query.Where(p => p.CECU_NM_NOME == nome);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public PLANO_CONTA GetItemById(Int32 id)
        {
            IQueryable<PLANO_CONTA> query = Db.PLANO_CONTA;
            query = query.Where(p => p.CECU_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PLANO_CONTA> GetAllItens(Int32 idAss)
        {
            IQueryable<PLANO_CONTA> query = Db.PLANO_CONTA.Where(p => p.CECU_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<PLANO_CONTA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<PLANO_CONTA> query = Db.PLANO_CONTA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<PLANO_CONTA> GetAllDespesas(Int32 idAss)
        {
            IQueryable<PLANO_CONTA> query = Db.PLANO_CONTA.Where(p => p.CECU_IN_ATIVO == 1);
            query = query.Where(p => p.CECU_IN_TIPO == 2);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<PLANO_CONTA> GetAllReceitas(Int32 idAss)
        {
            IQueryable<PLANO_CONTA> query = Db.PLANO_CONTA.Where(p => p.CECU_IN_ATIVO == 1);
            query = query.Where(p => p.CECU_IN_TIPO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<PLANO_CONTA> ExecuteFilter(Int32? grupoId, Int32? subGrupoId, Int32? tipo, Int32? movimento, String numero, String nome, Int32 idAss)
        {
            List<PLANO_CONTA> lista = new List<PLANO_CONTA>();
            IQueryable<PLANO_CONTA> query = Db.PLANO_CONTA;
            if (grupoId != null)
            {
                query = query.Where(p => p.GRCC_CD_ID == grupoId);
            }
            if (subGrupoId != null)
            {
                query = query.Where(p => p.SGCC_CD_ID == subGrupoId);
            }
            if (tipo != null)
            {
                query = query.Where(p => p.CECU_IN_TIPO == tipo);
            }
            if (movimento != null)
            {
                query = query.Where(p => p.CECU_IN_MOVTO == movimento);
            }
            if (!String.IsNullOrEmpty(numero))
            {
                query = query.Where(p => p.CECU_NR_NUMERO == numero);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.CECU_NM_NOME.Contains(nome));
            }
            if (query != null)
            {
                query = query.Where(p => p.CECU_IN_ATIVO == 1);
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CECU_NM_NOME);
                lista = query.ToList<PLANO_CONTA>();
            }
            return lista;
        }

    }
}
 