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
    public class FormaPagRecService : ServiceBase<FORMA_PAGTO_RECTO>, IFormaPagRecService
    {
        private readonly IFormaPagRecRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IContaBancariaRepository _conRepository;
        protected Db_PrecificacaoEntities Db = new Db_PrecificacaoEntities();

        public FormaPagRecService(IFormaPagRecRepository baseRepository, ILogRepository logRepository, IContaBancariaRepository conRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _conRepository = conRepository;

        }

        public FORMA_PAGTO_RECTO GetItemById(Int32 id)
        {
            FORMA_PAGTO_RECTO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<FORMA_PAGTO_RECTO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<FORMA_PAGTO_RECTO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public FORMA_PAGTO_RECTO CheckExist(FORMA_PAGTO_RECTO conta, Int32 idAss)
        {
            FORMA_PAGTO_RECTO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public List<FORMA_PAGTO_RECTO> ExecuteFilter(Int32? tipo, Int32? conta, String nome, Int32? idAss)
        {
            List<FORMA_PAGTO_RECTO> lista = _baseRepository.ExecuteFilter(tipo, conta, nome, idAss);
            return lista;
        }

        public List<CONTA_BANCO> GetAllContas(Int32 idAss)
        {
            return _conRepository.GetAllItens(idAss);
        }

        public Int32 Create(FORMA_PAGTO_RECTO item, LOG log)
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

        public Int32 Create(FORMA_PAGTO_RECTO item)
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


        public Int32 Edit(FORMA_PAGTO_RECTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FORMA_PAGTO_RECTO obj = _baseRepository.GetById(item.FOPR_CD_ID);
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

        public Int32 Edit(FORMA_PAGTO_RECTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FORMA_PAGTO_RECTO obj = _baseRepository.GetById(item.FOPR_CD_ID);
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

        public Int32 Delete(FORMA_PAGTO_RECTO item, LOG log)
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
