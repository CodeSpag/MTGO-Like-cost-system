# MTGO-Like Cost System

This is a console program I wrote to design and test the cost system for my digital card game. It was much easier to work with as a simple console program rather than designing it directly into my Unity project.

## What It Is

Have you ever played Magic: The Gathering? It has a unique cost system that most games nowadays do not use: five unique currencies and one generic currency. Everything costs a certain amount of unique currencies and possibly generic currency; unique currencies must be paid by matching currencies, and generic can be paid by any currency.

Personally, I love this system; it brings just the right amount of difficulty and necessary planning to be enjoyable. The learning curve is also very smooth. And if you don't like it, you can design your deck to use only one currency!

## Design Choices

I tried multiple versions and finally settled on storing the cost as a string and writing a series of functions to interpret the cost and process the payment. This approach provides the most flexibility and expandability. One of the things I tried was to store the cost as an array of size six; one position for each type. As a base, it's great, simple, and we are still dealing with numbers.

However, I couldn't figure out a simple way to implement hybrid cost/payment directly into the cost system (an ability would produce a hybrid currency that can pay for multiple colors but can be used only once; i.e., 1 USD/CAD could be used for either CAD or USD, but not both). Since this hybrid system is imperative, I decided to forego this idea and use the MTG text representation of costs.

If you're not familiar with this, here's a quick example of a cost and its notation: a card costs 2 generic mana, 2 white mana, and 1 red mana, resulting in a simple and clear 2WWR. Numbers are generic costs that can be paid with any color; letters must be paid by mana of that color. I represent hybrid sets like this: /WR\\, meaning that if it's a cost, it can be paid by any of the letters inside the /\\, and if it's a payment, any letter can be used to pay for the corresponding cost, then the set gets discarded.

## How It Works

This is just the core functionality of the system. There's more to it in the actual integration, like checks to see if we can afford a cost before paying for it.

In a nutshell, the overhead function "HandlePayment" processes a cost and a payment, then outputs the "change", making possible operations like "3WRBG - 1RB = 2WG." First, it tests if the input is valid and exits if it isn't. Second, it splits the string into three parts: generic, colors, and hybrid. Third, it processes each part accordingly; colors are paid first, then hybrid, then generic. Colors are rigid, so they are paid first, then hybrid, which is more flexible, then generic, which can be paid with any leftovers. Lastly, it recombines the "change" and returns it.

## What I've Learned

I learned a great deal about arrays, what they can and can't do, and how stretching their use leads to very complicated nested loops. With so many moving parts, I had to have a clear path to follow, so designing first and writing pseudo-code helped me a lot to get this working. I also had to make good use of dictionaries and lists to process the strings.

## Conclusion

I'm happy with the results; it's functional and I was able to integrate it into my Unity project without a hitch. I would love to have any feedback on this. I'm sure there are clever solutions I overlooked and ways to simplify or improve the code. Thanks for looking at my project!

