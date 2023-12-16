import { useContext, useEffect, useState } from "react"

import { Link } from "react-router-dom";

import { getOwnedCardsSeperate, getOwnedCardsStack } from "../../api/apiUserCard";

import { AppData } from "../../Root";
import CardDetails from "../Shared/CardDetails";
import Pagination from "../Shared/Pagination";
import ReusableCard from "../Shared/ReusableCard";

import './../../styles/User.css'

export default function UserCards() {

  const { userData } = useContext(AppData)

  const [cardOwned, setCardOwned] = useState([]);

  const [displayCards, setDisplayCards] = useState([]);
  const [isOpen, setIsOpen] = useState(false);
  const [cardSelected, setCardSelected] = useState();
  const [currentPage, setCurrentPage] = useState(1);
  const [isStack, setIsStack] = useState();

  const handleOpenDetail = (card) => {
    if (card.quantity > 0) {
      setIsOpen(true)
      setCardSelected(card);
    }
  }

  const handleToggleViewOption = () => {
    setIsStack(!isStack);
    setCurrentPage(1);
  }

  useEffect(() => {
    if (isStack) {
      getOwnedCardsStack(userData.username).then(data => {
        setCardOwned(data);
      })
    } else {
      getOwnedCardsSeperate(userData.username).then(data => {
        setCardOwned(data);
        setIsStack(false);
      })
    }
  }, [isStack])

  return (
    <>
      <div className="user-anything-wrapper">
        <div className="user-anything-title">
          <div className="your-cards-wrapper">
            <p>
              <span className="text-secondary">Your </span>
              <span className="text-primary">Cards</span>
            </p>
            <div className="user-cards-view-option" onClick={handleToggleViewOption}>
              <div className={`${!isStack ? 'cards-stack' : 'cards-seperate'} icon-8`}>
              </div>
              {!isStack ?
                <span className="text-third">Stack</span> :
                <span className="text-third">Seperate</span>
              }
            </div>
          </div>
          <Link to={'/user/cards'} className="link">Mange your cards</Link>
        </div>
        <div className="user-anything-container">
          {cardOwned.length ? displayCards.map((card, index) =>
            <ReusableCard card={card} key={index} onClick={() => handleOpenDetail(card)}/>
          ) : 
            <p className="no-data-text">
              <span>You're not having any cards. Let's </span>
              <Link className="link" to={"/"}>Buy Some</Link>
              <span> or </span>
              <Link className="link" to={"/gacha"}>Gacha</Link>
            </p>
          }
        </div>
        <div className="user-anything-footer">
          <Pagination currentPage={currentPage} list={cardOwned} numberItem={10} setCurrentPage={setCurrentPage} setPagedList={setDisplayCards} />
        </div>
      </div>
      <CardDetails isOpen={isOpen} selectedCard={cardSelected} onClose={() => setIsOpen(false)} />
    </>

  )
}
