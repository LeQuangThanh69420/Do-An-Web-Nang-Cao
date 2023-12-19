import { useContext, useEffect, useState } from 'react'
import Input from '../Shared/Input/Input'
import SearchAllCards from '../Shared/SearchSelections/SearchAllCards';

import { getOwnedCardsSeperate } from '../../api/apiUserCard';
import './../../styles/UserAllDeals.css'
import { AppData } from '../../Root';
import { createDeal, editDeal } from '../../api/apiDeal';

export default function DealModal({ isOpen, setIsOpen, title, chosingCards, setChosingCards, editedDeal, setEditedDeal, setDeals }) {
  const { userData, setType, setMessage, showToast } = useContext(AppData)

  const [inputValue, setInputValue] = useState('');
  const [chosenCard, setChosenCard] = useState({});
  const [searchObject, setSearchObject] = useState({
    cardName: "",
    cardTypeName: "",
    cardOriginName: "",
    cardElementName: "",
    cardRarityName: "",
  });

  const handleSearch = () => {
    getOwnedCardsSeperate(
      userData.username,
      searchObject.cardName,
      searchObject.cardTypeName,
      searchObject.cardOriginName,
      searchObject.cardElementName,
      searchObject.cardRarityName
    ).then((data) => {
      setChosingCards(data);
    });
  };

  const handleClose = () => {
    setChosenCard({});
    setIsOpen(false);
    setEditedDeal(undefined);
    setInputValue('');
  }

  const handleOK = async () => {
    if (inputValue && chosenCard.userCardId) {
      let response;
      if (editedDeal) {
        response = await editDeal(userData.username, editedDeal.dealId, chosenCard.userCardId, inputValue);
      } else {
        response = await createDeal(userData.username, chosenCard.userCardId, inputValue)
      }
      response.json().then(data => {
        if (response.status === 200) {
          setType('toast-success')
          handleClose();
          setDeals();
          setEditedDeal(undefined);
        } else {
          setType('toast-error')
        }
        setMessage(data.message);
        showToast();
      })
    }
  }

  const filterCards = (card) => {
    if (editedDeal) {
      return true;
    } else {
      return !card.onDeal;
    }
  }

  useEffect(() => {
    console.log('setting init');
    if (editedDeal) {
      setInputValue(editedDeal.price);
      setChosenCard(editedDeal);
    }
  }, [editedDeal])

  return (
    <>
      {isOpen && <div className='deal-modal-screen' onClick={handleClose}>
        <div className='deal-modal-container' onClick={(event) => { event.stopPropagation() }}>
          <div className='close-icon icon-5' onClick={handleClose}></div>
          <p className='deal-modal-title'>{title}</p>
          <div className='deal-modal-body'>
            <div className='deal-modal-inputs'>
              <div className='deal-price-wrapper'>
                <Input value={inputValue} label={"Enter price for this deal"} type={"number"} setData={setInputValue} />
              </div>
              <SearchAllCards searchObject={searchObject} setData={setSearchObject} onSearch={handleSearch} />
            </div>
            <div className='deal-modal-card-choser'>
              {chosingCards.filter(card => filterCards(card)).map(card =>
                <div key={card.userCardId} className='deal-modal-card-wrapper' onClick={() => setChosenCard(card)}>
                  <img src={card.cardImageURL} className={`${chosenCard.userCardId === card.userCardId ? 'chosen-card-for-deal' : ''} deal-modal-card-img`} />
                  {chosenCard.userCardId === card.userCardId && <svg xmlns="http://www.w3.org/2000/svg" height="20" width="20" className='chosen-card-icon' viewBox="0 0 512 512">
                    <path fill="#7400cc" d="M256 512A256 256 0 1 0 256 0a256 256 0 1 0 0 512zM369 209L241 337c-9.4 9.4-24.6 9.4-33.9 0l-64-64c-9.4-9.4-9.4-24.6 0-33.9s24.6-9.4 33.9 0l47 47L335 175c9.4-9.4 24.6-9.4 33.9 0s9.4 24.6 0 33.9z" />
                  </svg>}
                </div>
              )}
            </div>
          </div>
          <div className='deal-modal-buttons'>
            <button className='button-2' onClick={handleClose}>Cancel</button>
            <button onClick={handleOK}>OK</button>
          </div>
        </div>
      </div>}
    </>
  )
}