import React from 'react'
import './Explore.css'

interface Explore {
    username: string;
    liked: number;
    viewed: number;
    img: string;
    topic: string[];
}

const Explore : React.FC<Explore> = ({username, liked, viewed, img, topic}) => {
  return (
    <div id='explore_card'>
        <div className="image-container">
            <img src={img} alt="" />
        </div>
        <div className='info'>
            <span>{username}</span>
            <span>
                <span></span>
                <span></span>
            </span>
        </div>
    </div>
  )
}

export default Explore