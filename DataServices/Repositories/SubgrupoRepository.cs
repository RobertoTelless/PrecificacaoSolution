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
    public class SubgrupoRepository : RepositoryBase<SUBGRUPO_PLANO_CONTA>, ISubgrupoRepository
    {
        public SUBGRUPO_PLANO_CONTA CheckExist(SUBGRUPO_PLANO_CONTA conta, Int32 idAss)
        {
            IQueryable<SUBGRUPO_PLANO_CONTA> query = Db.SUBGRUPO_PLANO_CONTA;
            query = query.Where(p => p.SGCC_NR_NUMERO == conta.SGCC_NR_NUMERO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public SUBGRUPO_PLANO_CONTA GetItemById(Int32 id)
        {
            IQueryable<SUBGRUPO_PLANO_CONTA> query = Db.SUBGRUPO_PLANO_CONTA;
            query = query.Where(p => p.SGCC_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<SUBGRUPO_PLANO_CONTA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<SUBGRUPO_PLANO_CONTA> query = Db.SUBGRUPO_PLANO_CONTA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<SUBGRUPO_PLANO_CONTA> GetAllItens(Int32 idAss)
        {
            IQueryable<SUBGRUPO_PLANO_CONTA> query = Db.SUBGRUPO_PLANO_CONTA.Where(p => p.SGCC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 