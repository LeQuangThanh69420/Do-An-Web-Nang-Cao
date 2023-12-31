import dateTimeFormat from "./dateTimeFormat"

export const renderDealsRow = (item, index) => {
    return {
        no: <span className="list-row-no">{index + 1}</span>,
        cardName: (<div className="list-row-deal-info">
            <img src={item.cardImageURL} className="list-row-img"/>
            <div className="list-row-deal-info-texts">
                <p>
                    <span className="list-row-title">Name: </span>
                    <span className="list-row-value">{item.cardName}</span>
                </p>
                <div className="list-row-deal-info-riel">
                    <span className="list-row-title">Rarity: </span>
                    <span className="list-row-value">{item.cardRarityName}</span>
                </div>
            </div>
        </div>),
        acceptedDate: <span>{dateTimeFormat(item.acceptedDate).date} {dateTimeFormat(item.acceptedDate).time}</span>,
        username: item.sellUsername ? item.sellUsername : item.buyUsername,
        price: (
            <div className="list-row-deal-price">
                <div className="riu-coin-icon icon-9"></div>
                <span className="text-sixth list-row-deal-price-text">{item.price}</span>
            </div>
        )
    }
}