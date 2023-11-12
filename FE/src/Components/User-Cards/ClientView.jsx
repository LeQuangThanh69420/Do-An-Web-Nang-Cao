import Header from "./Header"
import Body from "./Body"
import AdSlider from "../AdSlider/Adslider";
import { useEffect, useState } from "react";
import './../../styles/ClientView.css'
import { useNavigate } from "react-router-dom";

function ClientView() {
    const navigate = useNavigate();

    const [cards, setCards] = useState([]);
    const [userData, setUserData] = useState(null);

    useEffect(() => {
        setUserData(JSON.parse(localStorage.getItem('userData')));
    }, [])

    return(
        <div className="main-content">
            <Header setCards={setCards} userData={userData}/>
            <AdSlider />
            <Body cards={cards} setCards={setCards}/>
        </div>
    ) 
}

export default ClientView