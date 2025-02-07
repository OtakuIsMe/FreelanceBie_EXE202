import React from 'react'
import './Specialities.css'

interface Special{
    type: string;
    img: string;
}

const Specialities : React.FC<Special> = ({type, img}) => {

    return (
        <div id='spec_card'>
            <div className="img-container">
                <img src={img} alt={type} />
            </div>
            <p>{type}</p>
        </div>
    )
}

export default Specialities