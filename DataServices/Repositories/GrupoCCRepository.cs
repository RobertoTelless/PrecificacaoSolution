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
    public class GrupoCCRepository : RepositoryBase<GRUPO_PLANO_CONTA>, IGrupoCCRepository
    {
        public GRUPO_PLANO_CONTA CheckExist(GRUPO_PLANO_CONTA conta, Int32 idAss)
        {
            IQueryable<GRUPO_PLANO_CONTA> query = Db.GRUPO_PLANO_CONTA;
            query = query.Where(p => p.GRCC_NR_NUMERO == conta.GRCC_NR_NUMERO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public GRUPO_PLANO_CONTA GetItemById(Int32 id)
        {
            IQueryable<GRUPO_PLANO_CONTA> query = Db.GRUPO_PLANO_CONTA;
            query = query.Where(p => p.GRCC_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<GRUPO_PLANO_CONTA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<GRUPO_PLANO_CONTA> query = Db.GRUPO_PLANO_CONTA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<GRUPO_PLANO_CONTA> GetAllItens(Int32 idAss)
        {
            IQueryable<GRUPO_PLANO_CONTA> query = Db.GRUPO_PLANO_CONTA.Where(p => p.GRCC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 