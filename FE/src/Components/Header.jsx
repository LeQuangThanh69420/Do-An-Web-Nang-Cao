import { useEffect, useState } from "react";
import "./../styles/Header.css";
import SearchSelection from "./SearchSelection/SearchSelection";
import { Link } from "react-router-dom";

function Header({ setCards }) {
    const [inputNameValue, setInputNameValue] = useState("");
    const [types, setTypes] = useState([]);
    const [origins, setOrigins] = useState([]);
    const [elements, setElements] = useState([]);
    const [rarities, setRarities] = useState([]);

    const [searchObject, setSearchObject] = useState({
        name: "",
        type: "",
        origin: "",
        element: "",
        rarity: "",
    });

    useEffect(() => {
        fetch(
            `http://localhost:5233/api/Card/searchCard?CardName=${searchObject.name}&CardTypeName=${searchObject.type}&CardOriginName=${searchObject.origin}&CardElementName=${searchObject.element}&CardRarityName=${searchObject.rarity}`
        )
            .then((res) => res.json())
            .then((data) => {
                setCards(data);
            });
    }, [searchObject]);

    useEffect(() => {
        console.log(searchObject);
    }, [searchObject]);

    //get types
    useEffect(() => {
        fetch("http://localhost:5233/api/Card/getCardType")
            .then((res) => res.json())
            .then((data) => {
                setTypes(data);
            });
    }, []);
    //get origins
    useEffect(() => {
        fetch("http://localhost:5233/api/Card/getCardOrigin")
            .then((res) => res.json())
            .then((data) => {
                //console.log(data);
                setOrigins(data);
            });
    }, []);
    //get elements
    useEffect(() => {
        fetch("http://localhost:5233/api/Card/getCardElement")
            .then((res) => res.json())
            .then((data) => {
                setElements(data);
            });
    }, []);
    //get rarities
    useEffect(() => {
        fetch("http://localhost:5233/api/Card/getCardRarity")
            .then((res) => res.json())
            .then((data) => {
                setRarities(data);
            });
    }, []);

    const handleAddName = () => {
        setSearchObject({
            ...searchObject,
            name: inputNameValue,
        });
    };
    const updateSearchObject = (propSet, value) => {
        setSearchObject({
            ...searchObject,
            [propSet]: value,
        });
    };

    return (
        <div className="main-container">
            <div className="header-bar">
                <div className="logo">
                    <svg
                        version="1.0"
                        xmlns="http://www.w3.org/2000/svg"
                        width="40px"
                        height="40px"
                        viewBox="0 0 440.000000 440.000000"
                        preserveAspectRatio="xMidYMid meet"
                    >
                        <g
                            transform="translate(0.000000,440.000000) scale(0.100000,-0.100000)"
                            fill="#000000"
                            stroke="none"
                        >
                            <path
                                d="M2025 4384 c-305 -32 -549 -100 -800 -224 -432 -212 -807 -595 -1006
-1027 -23 -48 -45 -90 -49 -93 -10 -7 -84 -226 -109 -325 -30 -121 -51 -282
-50 -386 0 -52 2 -89 5 -84 2 6 12 69 23 141 27 190 86 316 216 469 102 119
285 239 440 289 450 145 963 -73 1168 -496 l42 -87 285 -1 286 0 33 68 c118
239 326 423 572 507 144 50 209 55 655 55 401 0 414 1 414 19 0 11 -4 21 -9
23 -5 2 -32 42 -60 90 -116 197 -318 431 -496 575 -303 245 -677 411 -1055
468 -102 15 -423 27 -505 19z"
                            />
                            <path
                                d="M890 2901 c-52 -8 -201 -77 -268 -123 -91 -63 -212 -190 -212 -224 0
-12 91 -14 605 -14 376 0 605 4 605 10 0 5 -6 13 -14 17 -8 4 -35 36 -60 70
-61 83 -138 142 -272 209 l-110 55 -115 4 c-63 2 -134 0 -159 -4z"
                            />
                            <path
                                d="M3405 2880 c-164 -14 -271 -53 -391 -144 -52 -38 -120 -115 -148
-163 -6 -10 146 -13 744 -13 l751 0 -7 38 c-8 50 -31 143 -55 225 l-20 67
-392 -1 c-216 -1 -432 -5 -482 -9z"
                            />
                            <path
                                d="M10 2178 c0 -53 29 -221 39 -228 4 -3 19 -39 34 -81 140 -387 542
-651 956 -626 201 12 404 89 557 211 123 99 235 246 285 374 l24 62 283 0
c278 0 284 0 289 -20 3 -12 21 -53 39 -91 119 -247 325 -426 594 -515 82 -28
92 -29 290 -29 198 0 208 1 290 29 263 87 468 262 587 501 58 116 103 289 103
396 l0 59 -2185 0 -2185 0 0 -42z m3142 -306 c90 -77 177 -106 306 -100 108 5
171 28 253 93 l44 35 133 0 134 0 -21 -32 c-127 -202 -349 -328 -576 -328
-227 0 -449 126 -576 328 l-21 32 146 0 c145 0 145 0 178 -28z m-2440 -11 c83
-72 197 -106 328 -99 84 5 158 35 231 93 l44 35 148 0 c81 0 147 -3 147 -7 0
-20 -112 -153 -165 -196 -118 -95 -289 -157 -435 -157 -146 0 -317 62 -435
157 -53 43 -165 176 -165 196 0 4 61 7 135 7 134 0 134 0 167 -29z"
                            />
                            <path
                                d="M2102 1751 c-60 -21 -122 -75 -155 -136 -21 -38 -22 -54 -25 -362
l-3 -323 270 0 271 0 0 298 c0 357 -4 379 -84 458 -79 80 -170 101 -274 65z"
                            />
                            <path
                                d="M276 1153 c110 -199 211 -332 378 -499 339 -339 751 -548 1226 -621
159 -24 482 -24 640 0 393 61 759 222 1065 470 188 152 390 390 514 605 l61
107 -178 3 c-155 2 -178 1 -187 -14 -93 -153 -321 -401 -465 -506 -452 -330
-973 -447 -1505 -338 -471 97 -914 396 -1176 794 l-43 66 -184 0 -184 0 38
-67z"
                            />
                        </g>
                    </svg>
                    DuRiu
                </div>
                <div className="search-bar">
                    <span>Search</span>
                    <input
                        type="text"
                        name=""
                        id=""
                        placeholder="Enter card name"
                        value={inputNameValue}
                        onChange={(event) => setInputNameValue(event.target.value)}
                    />
                    <button onClick={handleAddName}>Search</button>
                </div>
                <div className="users-button">
                    <Link to={"/login"}>Login</Link>
                </div>
            </div>
            <div className="search-opt-container">
                <div className="search-opt">
                    <SearchSelection
                        type="Type"
                        selections={types}
                        propSet="type"
                        onSelect={updateSearchObject}
                    />
                    <SearchSelection
                        type="Origin"
                        selections={origins}
                        propSet="origin"
                        onSelect={updateSearchObject}
                    />
                    <SearchSelection
                        type="Element"
                        selections={elements}
                        propSet="element"
                        onSelect={updateSearchObject}
                    />
                    <SearchSelection
                        type="Rarity"
                        selections={rarities}
                        propSet="rarity"
                        onSelect={updateSearchObject}
                    />
                </div>
            </div>
        </div>
    );
}

export default Header;
