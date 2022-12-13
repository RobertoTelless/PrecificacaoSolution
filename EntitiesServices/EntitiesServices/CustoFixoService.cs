using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class CustoFixoService : ServiceBase<CUSTO_FIXO>, ICustoFixoService
    {
        private readonly ICustoFixoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ICategoriaCustoFixoRepository _tipoRepository;
        private readonly IPeriodicidadeRepository _perRepository;
        protected Db_PrecificacaoEntities Db = new Db_PrecificacaoEntities();

        public CustoFixoService(ICustoFixoRepository baseRepository, ILogRepository logRepository, ICategoriaCustoFixoRepository tipoRepository, IPeriodicidadeRepository perRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _perRepository = perRepository;
        }

        public CUSTO_FIXO CheckExist(CUSTO_FIXO conta, Int32 idAss)
        {
            CUSTO_FIXO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public CUSTO_FIXO GetItemById(Int32 id)
        {
            CUSTO_FIXO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<CUSTO_FIXO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<CUSTO_FIXO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<CATEGORIA_CUSTO_FIXO> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public List<CUSTO_FIXO> ExecuteFilter(Int32? catId, String nome, DateTime? dataInicio, DateTime? dataFinal, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(catId, nome, dataInicio, dataFinal, idAss);
        }

        public Int32 Create(CUSTO_FIXO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(CUSTO_FIXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public Int32 Edit(CUSTO_FIXO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CUSTO_FIXO obj = _baseRepository.GetById(item.CUFX_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(CUSTO_FIXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CUSTO_FIXO obj = _baseRepository.GetById(item.CUFX_CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Delete(CUSTO_FIXO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

    }
}
